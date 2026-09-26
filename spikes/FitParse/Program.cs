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
