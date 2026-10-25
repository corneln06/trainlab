using System.Net;
using System.Net.Http.Json;

using Shouldly;

using TrainLab.IntegrationTests.Infrastructure;

namespace TrainLab.IntegrationTests;

[Collection(ApiCollection.Name)]
public class ProfileTests(TrainLabApiFactory factory)
{
    private static readonly object ValidProfile = new
    {
        sex = "Male",
        birthYear = 2002,
        weightKg = 75.0,
        restingHr = 49,
        maxHr = 195,
        timeZone = "Europe/Copenhagen",
    };

    private sealed record ProfileDto(
        string Sex, int? BirthYear, double? WeightKg, int? RestingHr, int? MaxHr, string TimeZone);

    [Fact]
    public async Task New_user_gets_default_profile()
    {
        var client = await factory.CreateLoggedInClientAsync();

        var profile = await client.GetFromJsonAsync<ProfileDto>("/api/profile");

        profile.ShouldNotBeNull();
        profile.RestingHr.ShouldBeNull();
        profile.MaxHr.ShouldBeNull();
        profile.TimeZone.ShouldBe("Europe/Copenhagen");
    }

    [Fact]
    public async Task Saved_profile_is_returned()
    {
        var client = await factory.CreateLoggedInClientAsync();

        (await client.PutAsJsonAsync("/api/profile", ValidProfile)).EnsureSuccessStatusCode();
        var profile = await client.GetFromJsonAsync<ProfileDto>("/api/profile");

        profile.ShouldNotBeNull();
        profile.RestingHr.ShouldBe(49);
        profile.MaxHr.ShouldBe(195);
        profile.Sex.ShouldBe("Male");
    }

    [Fact]
    public async Task Resting_hr_above_max_hr_is_rejected()
    {
        var client = await factory.CreateLoggedInClientAsync();
        var invalid = new { sex = "Male", restingHr = 150, maxHr = 140, timeZone = "Europe/Copenhagen" };

        var response = await client.PutAsJsonAsync("/api/profile", invalid);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain("RestingHr");
    }

    [Fact]
    public async Task Users_cannot_see_each_others_profile()
    {
        var alice = await factory.CreateLoggedInClientAsync();
        var bob = await factory.CreateLoggedInClientAsync();

        (await alice.PutAsJsonAsync("/api/profile", ValidProfile)).EnsureSuccessStatusCode();
        var bobsProfile = await bob.GetFromJsonAsync<ProfileDto>("/api/profile");

        bobsProfile.ShouldNotBeNull();
        bobsProfile.RestingHr.ShouldBeNull();
    }

    [Fact]
    public async Task Profile_requires_login()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/profile");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}