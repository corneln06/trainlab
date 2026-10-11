using Microsoft.EntityFrameworkCore;

namespace TrainLab.Modules.Maps.Persistence;

public class MapsDbContext(DbContextOptions<MapsDbContext> options) : DbContext(options)
{
    public const string Schema = "maps";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MapsDbContext).Assembly);
    }
}