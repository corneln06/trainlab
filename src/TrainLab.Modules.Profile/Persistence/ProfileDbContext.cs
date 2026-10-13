using Microsoft.EntityFrameworkCore;

namespace TrainLab.Modules.Profile.Persistence;

public class ProfileDbContext(DbContextOptions<ProfileDbContext> options) : DbContext(options)
{
    public const string Schema = "profile";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileDbContext).Assembly);
    }
}