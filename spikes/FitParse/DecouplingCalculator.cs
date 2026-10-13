
using Dynastream.Fit;

namespace FitParse;

public record DecouplingResult(
    bool isEligible,
    String? Reason,
    double? EfFirstHalf,
    double? EfSecondHalf,
    double? DecouplingPercent)
{
    public static DecouplingResult NotApplicable(String reason) =>
        new(false, reason, null, null, null);
}


public static class DecouplingCalculator
{
    public const double MinDurationMinutes = 45;
    public const double WarmingMinutes = 10;
    public const double MinMovingSpeed = 1.0;

    private record Sample(System.DateTime Time, double Speed, double Hr);


    public static DecouplingResult Calculate(List<RecordMesg?> records)
    {
        var samples = new List<Sample>();
        foreach (var r in records)
        {
            var time = r?.GetTimestamp()?.GetDateTime();
            var speed = r?.GetEnhancedSpeed();
            var hr = r?.GetHeartRate();

            if (time is null || hr is null || speed is null || hr == 0)
            {
                continue;
            }

            if (speed < MinMovingSpeed)
            {
                continue;
            }

            samples.Add(new Sample(time.Value, speed.Value, hr.Value));

        }

        if (samples.Count < 2)
        {
            return DecouplingResult.NotApplicable(
                "Not enough samples with speed and heart rate");
        }

        var start = samples[0].Time;
        var end = samples[^1].Time;
        var durationMinutes = (end - start).TotalMinutes;

        if (durationMinutes < MinDurationMinutes)
        {
            return DecouplingResult.NotApplicable($"Too short: {durationMinutes:F0} min (needs {MinDurationMinutes} min)");
        }

        var steadyStart = start.AddMinutes(WarmingMinutes);
        var steady = samples.Where(s => s.Time >= steadyStart).ToList();

        var middle = steadyStart + (end - steadyStart) / 2;
        var firstHalf = steady.Where(s => s.Time < middle).ToList();
        var secondHalf = steady.Where(s => s.Time >= middle).ToList();

        if (firstHalf.Count == 0 || secondHalf.Count == 0)
        {
            return DecouplingResult.NotApplicable(
                "Could not split the session into two halves");
        }
        double efFirst = EfficiencyFactor(firstHalf);
        double efSecond = EfficiencyFactor(secondHalf);
        double decoupling = (efFirst - efSecond) / efFirst * 100;

        return new DecouplingResult(true, null, efFirst, efSecond, decoupling);
    }
    private static double EfficiencyFactor(List<Sample> half) =>
        half.Average(s => s.Speed) / half.Average(s => s.Hr);
}