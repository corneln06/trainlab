using System.Net;

namespace TrainLab.IntegrationTests;

using Shouldly;

using TrainLab.IntegrationTests.Infrastructure;


[Collection(ApiCollection.Name)]
public class HealthTests(TrainLabApiFactory factory)
{
    [Fact]
    public async Task Health_endpoint_reports_healthy_database()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).ShouldBe("Healthy");
    }

}