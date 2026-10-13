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
}