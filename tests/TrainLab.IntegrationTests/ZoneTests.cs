using System.Net;
using System.Net.Http.Json;

using Shouldly;

using TrainLab.IntegrationTests.Infrastructure;

namespace TrainLab.IntegrationTests;

[Collection(ApiCollection.Name)]
public class ZoneTests(TrainLabApiFactory factory)
{
    private sealed record ZoneDto(Guid Id, string Sport, int Lthr, DateOnly EffectiveFrom);

    private static Task<HttpResponseMessage> CreateZoneAsync(
        HttpClient client, string sport, int lthr, string effectiveFrom) =>
        client.PostAsJsonAsync("/api/zones", new { sport, lthr, effectiveFrom });

    [Fact]
    public async Task Created_zone_sets_are_listed()
    {
        var client = await factory.CreateLoggedInClientAsync();

        (await CreateZoneAsync(client, "Running", 176, "2026-01-01")).StatusCode.ShouldBe(HttpStatusCode.Created);
        (await CreateZoneAsync(client, "Running", 181, "2026-09-15")).StatusCode.ShouldBe(HttpStatusCode.Created);

        var zones = await client.GetFromJsonAsync<List<ZoneDto>>("/api/zones");

        zones.ShouldNotBeNull();
        zones.Select(z => z.Lthr).ShouldBe([176, 181]);
    }

    [Theory]
    [InlineData("2025-12-31", null)] 
    [InlineData("2026-01-01", 176)] 
    [InlineData("2026-03-01", 176)]   
    [InlineData("2026-09-15", 181)]   
    [InlineData("2026-11-01", 181)]   
    public async Task Effective_zone_set_depends_on_the_date(string date, int? expectedLthr)
    {
        var client = await factory.CreateLoggedInClientAsync();
        await CreateZoneAsync(client, "Running", 176, "2026-01-01");
        await CreateZoneAsync(client, "Running", 181, "2026-09-15");

        var response = await client.GetAsync($"/api/zones/effective?sport=Running&date={date}");

        if (expectedLthr is null)
        {
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            return;
        }

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var zone = await response.Content.ReadFromJsonAsync<ZoneDto>();
        zone.ShouldNotBeNull();
        zone.Lthr.ShouldBe(expectedLthr.Value);
    }

    [Fact]
    public async Task Zone_set_in_the_future_is_rejected()
    {
        var client = await factory.CreateLoggedInClientAsync();
        var nextYear = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1).ToString("yyyy-MM-dd");

        var response = await CreateZoneAsync(client, "Running", 181, nextYear);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Same_sport_and_date_twice_is_a_conflict()
    {
        var client = await factory.CreateLoggedInClientAsync();
        await CreateZoneAsync(client, "Running", 181, "2026-09-15");

        var response = await CreateZoneAsync(client, "Running", 185, "2026-09-15");

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Running_zones_do_not_answer_cycling_lookups()
    {
        var client = await factory.CreateLoggedInClientAsync();
        await CreateZoneAsync(client, "Running", 181, "2026-09-15");

        var response = await client.GetAsync("/api/zones/effective?sport=Cycling&date=2026-11-01");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Users_cannot_see_each_others_zone_sets()
    {
        var alice = await factory.CreateLoggedInClientAsync();
        var bob = await factory.CreateLoggedInClientAsync();
        await CreateZoneAsync(alice, "Running", 181, "2026-09-15");

        var bobsZones = await bob.GetFromJsonAsync<List<ZoneDto>>("/api/zones");
        var bobsLookup = await bob.GetAsync("/api/zones/effective?sport=Running&date=2026-11-01");

        bobsZones.ShouldNotBeNull();
        bobsZones.ShouldBeEmpty();
        bobsLookup.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}