using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TrainLab.Modules.Profile.Users;

namespace TrainLab.Modules.Profile.Athletes;

public class AthleteProfileConfiguration : IEntityTypeConfiguration<AthleteProfile>
{
    public void Configure(EntityTypeBuilder<AthleteProfile> builder)
    {

        builder.ToTable("athlete_profiles");
        builder.HasKey(p => p.UserId);

        builder.HasOne<User>().WithOne().HasForeignKey<AthleteProfile>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Sex).HasConversion<string>().HasMaxLength(16);

        builder.Property(p => p.TimeZone).HasMaxLength(64).IsRequired();





    }

}