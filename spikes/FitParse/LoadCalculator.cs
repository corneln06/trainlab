using Dynastream.Fit;

namespace FitParse;

public record LoadResult(double Trimp, double TrimpOneHourAtThreshold, double Load, double Minutes);

public static class LoadCalculator
{
    public const int HrRest = 49;
    public const int HrMax = 183;
    public const int Lthr = 174;
    public const double A = 0.64;
    public const double B = 1.92;
    public const double MaxGapSeconds = 30; 
    public static LoadResult Calculate(List<RecordMesg?> records)
    {
        double trimp = 0;
        double minutes = 0;
        System.DateTime? previousTime = null;

        foreach (var sample in records)
        {
            System.DateTime? time = sample?.GetTimestamp()?.GetDateTime();
            if (time is null)
            {
                continue;
            }
            double dtMinutes = 0;
            if (previousTime is not null)
            {
                double gapSeconds = (time.Value - previousTime.Value).TotalSeconds;
                dtMinutes = gapSeconds > MaxGapSeconds ? 0 : gapSeconds / 60.0;
            }
            previousTime = time;
            
            byte? hr = sample?.GetHeartRate();
            if (hr is null)
            {
                continue;
            }
            minutes += dtMinutes;
            trimp += Contribution(dtMinutes, HrReserve(hr.Value));
        }
        double trimpAtThreshold = Contribution(60, HrReserve(Lthr));
        
        double load = trimp / trimpAtThreshold * 100;

        return new LoadResult(trimp, trimpAtThreshold, load, minutes);
    }
    private static double HrReserve(double hr) =>
        (hr - HrRest) / (HrMax - HrRest);
    
    private static double Contribution(double dtMinutes, double hrr) =>
        dtMinutes * hrr * A * Math.Exp(B * hrr);
}