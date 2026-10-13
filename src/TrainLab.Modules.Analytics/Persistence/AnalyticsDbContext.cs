using Microsoft.EntityFrameworkCore;

namespace TrainLab.Modules.Analytics.Persistence;

public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options)
{
    public const string Schema = "analytics";

    protected override void OnModelCreating(
        ModelBuilder optionsBuilder)
    {
        base.OnModelCreating(optionsBuilder);

        optionsBuilder.HasDefaultSchema(Schema);
        optionsBuilder.ApplyConfigurationsFromAssembly(
            typeof(AnalyticsDbContext).Assembly);
    }

}