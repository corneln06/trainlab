using Shouldly;

using TrainLab.Modules.Profile.Zones;

namespace TrainLab.UnitTests.Zones;

public class ZoneCalculatorTests
{
    [Fact]
    public void Running_lthr_181_gives_the_reference_zones()
    {
        var zones = ZoneCalculator.Calculate(Sport.Running, 181);

        zones.ShouldBe(
        [
            new HeartRateZone("Z1", null, 153),
            new HeartRateZone("Z2", 154, 161),
            new HeartRateZone("Z3", 162, 170),
            new HeartRateZone("Z4", 171, 179),
            new HeartRateZone("Z5a", 180, 185),
            new HeartRateZone("Z5b", 186, 192),
            new HeartRateZone("Z5c", 193, null),
        ]);
    }

    [Theory]
    [InlineData(Sport.Running, 150)]
    [InlineData(Sport.Running, 181)]
    [InlineData(Sport.Running, 200)]
    [InlineData(Sport.Cycling, 150)]
    [InlineData(Sport.Cycling, 174)]
    [InlineData(Sport.Cycling, 200)]
    public void Zones_have_no_gaps_or_overlaps(Sport sport, int lthr)
    {
        var zones = ZoneCalculator.Calculate(sport, lthr);

        zones.Count.ShouldBe(7);
        zones[0].MinBpm.ShouldBeNull();
        zones[^1].MaxBpm.ShouldBeNull();

        for (var i = 1; i < zones.Count; i++)
        {
            zones[i].MinBpm.ShouldBe(zones[i - 1].MaxBpm + 1);
        }
    }

    [Fact]
    public void Cycling_z1_ends_lower_than_running_for_the_same_lthr()
    {
        var running = ZoneCalculator.Calculate(Sport.Running, 175);
        var cycling = ZoneCalculator.Calculate(Sport.Cycling, 175);

        cycling[0].MaxBpm!.Value.ShouldBeLessThan(running[0].MaxBpm!.Value);
    }
}