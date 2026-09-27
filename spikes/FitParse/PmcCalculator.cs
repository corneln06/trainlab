namespace FitParse;

public record DailyLoad(DateOnly Day,double LoadPerDay, double Tsb, double Ctl, double Atl);

public static class PmcCalculator
{
    public static IReadOnlyList<DailyLoad> ComputePmc(
        IReadOnlyDictionary<DateOnly, double> loadPerDay, DateOnly from,
        DateOnly to, double ctlStart = 0, double atlStart = 0)
    {
        var result = new List<DailyLoad>();
        double ctl = ctlStart, atl = atlStart;

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            var load = loadPerDay.GetValueOrDefault(day, 0);
            var tsb =  ctl - atl;
            ctl += (load - ctl) / 42.0;
            atl += (load - atl) / 7.0;
            result.Add(new DailyLoad(day, load, tsb, ctl, atl ));
        }

        return result;
    }
}