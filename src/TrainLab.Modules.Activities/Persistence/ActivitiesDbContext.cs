using Microsoft.EntityFrameworkCore;

namespace TrainLab.Modules.Activities.Persistence;

public class ActivitiesDbContext(DbContextOptions<ActivitiesDbContext> options) : DbContext(options)
{
    public const string Schema = "activities";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("timescaledb");
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActivitiesDbContext).Assembly);
    }
}