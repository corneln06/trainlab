using Microsoft.EntityFrameworkCore;

using TrainLab.Modules.Activities.Persistence;
using TrainLab.Modules.Analytics.Persistence;
using TrainLab.Modules.Import.Persistence;
using TrainLab.Modules.Maps.Persistence;
using TrainLab.Modules.Profile.Persistence;

namespace TrainLab.Api.Persistence;

public class DatabaseMigrator
{
    public static async Task MigrateAllAsync(
        IServiceProvider rootServiceProvider, CancellationToken ct = default)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();
        var services = scope.ServiceProvider;

        DbContext[] contexts =
        [
            services.GetRequiredService<ActivitiesDbContext>(),
            services.GetRequiredService<ProfileDbContext>(),
            services.GetRequiredService<ImportDbContext>(),
            services.GetRequiredService<AnalyticsDbContext>(),
            services.GetRequiredService<MapsDbContext>(),
        ];
        foreach (var context in contexts)
        {
            await context.Database.MigrateAsync(ct);
        }
    }
}