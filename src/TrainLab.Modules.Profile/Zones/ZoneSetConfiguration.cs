using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TrainLab.Modules.Profile.Users;

namespace TrainLab.Modules.Profile.Zones;

public class ZoneSetConfiguration : IEntityTypeConfiguration<ZoneSet>
{
    public void Configure(EntityTypeBuilder<ZoneSet> builder)
    {
        builder.ToTable("zone_sets");

        builder.HasKey(z => z.Id);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(z => z.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(z => z.Sport)
            .HasConversion<string>()
            .HasMaxLength(16);
        builder.HasIndex(z => new { z.UserId, z.Sport, z.EffectiveFrom })
            .IsUnique();
    }
}