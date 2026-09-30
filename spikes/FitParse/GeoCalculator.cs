using System.Text.Json;

using Dynastream.Fit;

namespace FitParse;

public static class GeoCalculator
{
    private const double SemicirclesToDegrees = 180.0 / 2147483648.0; 

    public static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6_371_000; // earth radius in metres
        double dLat = double.DegreesToRadians(lat2 - lat1);
        double dLon = double.DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(double.DegreesToRadians(lat1)) * Math.Cos(double.DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return 2 * R * Math.Asin(Math.Sqrt(a));
    }

    public static double TotalTrackDistance(List<RecordMesg?> records)
    {
        double total = 0;
        (double Lat, double Lon)? previous = null;

        foreach (var r in records)
        {
            int? latSemicircles = r?.GetPositionLat();
            int? lonSemicircles = r?.GetPositionLong();

            if (latSemicircles is null || lonSemicircles is null)
            {
                continue;
            }

            double lat = latSemicircles.Value * SemicirclesToDegrees;
            double lon = lonSemicircles.Value * SemicirclesToDegrees;
            if (previous is not null)
            {
                total += HaversineMeters(previous.Value.Lat, previous.Value.Lon, lat, lon);
            }

            previous = (lat, lon);
        }

        return total;
    }

    public static void ExportGeoJson(List<RecordMesg?> records, string path)
    {
        var coordinates = new List<double[]>();

        foreach (var r in records)
        {
            int? latSemicircles = r?.GetPositionLat();
            int? lonSemicircles = r?.GetPositionLong();

            if (latSemicircles is null || lonSemicircles is null)
            {
                continue;
            }

            double lat = latSemicircles.Value * SemicirclesToDegrees;
            double lon = lonSemicircles.Value * SemicirclesToDegrees;
            
            coordinates.Add(new[] { lon, lat });
        }

        var geoJson = new
        {
            type = "Feature",
            properties = new { },
            geometry = new
            {
                type = "LineString",
                coordinates
            }
        };

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        System.IO.File.WriteAllText(path, JsonSerializer.Serialize(geoJson));
    }
}