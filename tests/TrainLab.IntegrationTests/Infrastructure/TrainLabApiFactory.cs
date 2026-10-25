using System.Net.Http.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;

using TrainLab.Api.Persistence;


namespace TrainLab.IntegrationTests.Infrastructure;

public sealed class TrainLabApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("timescale/timescaledb-ha:pg17").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default",
            _database.GetConnectionString());
    }
    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        await DatabaseMigrator.MigrateAllAsync(Services);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _database.DisposeAsync();
        await base.DisposeAsync();
    }
    public async Task<HttpClient> CreateLoggedInClientAsync()
    {
        var client = CreateClient();
        var credentials = new { email = $"{Guid.NewGuid()}@test.local", password = "Test123!" };

        (await client.PostAsJsonAsync("/api/auth/register", credentials)).EnsureSuccessStatusCode();
        (await client.PostAsJsonAsync("/api/auth/login?useCookies=true", credentials)).EnsureSuccessStatusCode();

        return client;
    }
}