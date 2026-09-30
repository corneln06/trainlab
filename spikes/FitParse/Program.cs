using Dynastream.Fit;

using FitParse;

using File = System.IO.File;

if (args.Length == 0)
{
    return;
}
string fitFilePath = args[0];

using FileStream stream = File.OpenRead(fitFilePath);

var fitDecoder = new FitDecoder();

if (!fitDecoder.Decode(stream))
{
    Console.WriteLine("Invalid file format");
    return;
}
var message = fitDecoder.Messages;
Console.WriteLine($"File type: {message.FileId?.GetType()}, serial: {message.FileId?.GetSerialNumber()}");

foreach (var s in message.Sessions)
{
    Console.WriteLine(
        $"Session: {s.GetSport()} | start {s.GetStartTime()?.GetDateTime():u} | " +
        $"timer {s.GetTotalTimerTime():F0} s | distance {s.GetTotalDistance():F0} m | " +
        $"avg HR {s.GetAvgHeartRate()}");
}

double? Semicircles = (180.0 / Math.Pow(2, 31));

foreach (var v in message.Records.Take(10))
{
    if (v != null)
    {
        double? lat = v.GetPositionLat() * Semicircles;
        double? lon = v.GetPositionLong() * Semicircles;
        Console.WriteLine(
            $"{v.GetTimestamp()?.GetDateTime():HH:mm:ss}  HR {v.GetHeartRate()}  " +
            $"speed {v.GetEnhancedSpeed():F2} m/s  alt {v.GetEnhancedAltitude():F1} m  " +
            $"lat {lat:F6}  lon {lon:F6}");
    }
   
    
}
Console.WriteLine($"Records: {message.Records.Count}");
var resultLoad = LoadCalculator.Calculate(message.Records);

var RideDate = DateOnly.FromDateTime(message.Sessions[0].GetStartTime()!
    .GetDateTime().ToLocalTime());
var loadPerDay = new Dictionary<DateOnly, double>()
{
    [RideDate] = resultLoad.Load
};
var resultPmc = PmcCalculator.ComputePmc(loadPerDay, RideDate, RideDate.AddDays(30));

Console.WriteLine(
    $"TRIMP {resultLoad.Trimp:F2} | TRIMP_1h_LT {resultLoad.TrimpOneHourAtThreshold:F2} | " +
    $"Load {resultLoad.Load:F1} | Minutes {resultLoad.Minutes:F2}");
foreach (var day in resultPmc.Take(10))
{
    Console.WriteLine($"{day.Day}  load {day.LoadPerDay:F0}  CTL {day.Ctl:F1}  ATL {day.Atl:F1}  TSB {day.Tsb:F1}");
}    

var decoupling = DecouplingCalculator.Calculate(message.Records);

if (decoupling.isEligible)
{
    Console.WriteLine(
    $"Decoupling {decoupling.DecouplingPercent:F1}% " +
        $"(EF first half {decoupling.EfFirstHalf:F4}, second half {decoupling.EfSecondHalf:F4})");
}
else
{
    Console.WriteLine($"Decoupling not applicable: {decoupling.Reason}");
}
double gpsDistance = GeoCalculator.TotalTrackDistance(message.Records);
double deviceDistance = message.Sessions[0].GetTotalDistance() ?? 0;
double differencePercent = (gpsDistance - deviceDistance) / deviceDistance * 100;

Console.WriteLine(
    $"Distance: device {deviceDistance:F0} m | summed GPS {gpsDistance:F0} m | " +
    $"difference {differencePercent:+0.0;-0.0}%");
GeoCalculator.ExportGeoJson(message.Records, "spikes/map/public/track.geojson");
Console.WriteLine("Track exported to spikes/map/public/track.geojson");    