namespace TrainLab.Modules.Profile.Zones;

public class ZoneCalculator
{
    private static readonly string[] ZoneNames = ["Z1", "Z2", "Z3", "Z4", "Z5a", "Z5b", "Z5c"];

    private static readonly Dictionary<Sport, double> Z2Start = new()
    {
        [Sport.Running] = 0.85,
        [Sport.Cycling] = 0.81,
    };
    private static readonly Dictionary<Sport, double[]> UpperBounds = new()
    {
        [Sport.Running] = [0.89, 0.94, 0.99, 1.02, 1.06],
        [Sport.Cycling] = [0.89, 0.93, 0.99, 1.02, 1.06],
    };

    public static IReadOnlyList<HeartRateZone> Calculate(Sport sport, int lthr)
    {
        var maxes = new List<int> { Bpm(Z2Start[sport], lthr) - 1 };

        foreach (var fraction in UpperBounds[sport])
        {
            maxes.Add(Bpm(fraction, lthr));
        }
        var zones = new List<HeartRateZone>();

        for (var i = 0; i < ZoneNames.Length; i++)
        {
            int? min = i == 0 ? null : maxes[i - 1] + 1;
            int? max = i == ZoneNames.Length - 1 ? null : maxes[i];

            zones.Add(new HeartRateZone(ZoneNames[i], min, max));
        }
        return zones;

    }
    private static int Bpm(double fraction, int lthr) =>
        (int)Math.Round(fraction * lthr, MidpointRounding.AwayFromZero);
}