using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using TrainLab.Modules.Profile.Athletes;
using TrainLab.Modules.Profile.Users;
using TrainLab.Modules.Profile.Zones;

namespace TrainLab.Modules.Profile.Persistence;

public class ProfileDbContext(DbContextOptions<ProfileDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public const string Schema = "profile";
    public DbSet<AthleteProfile> AthleteProfiles => Set<AthleteProfile>();
    public DbSet<ZoneSet>  ZoneSets => Set<ZoneSet>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProfileDbContext).Assembly);
    }
}