namespace TrainLab.Modules.Profile.Athletes;

public enum Sex { Male, Female }
public class AthleteProfile
{
    public Guid UserId { get; set; }
    public Sex? Sex { get; set; }
    public int? BirthYear { get; set; }
    public double? WeightKg { get; set; }
    public int? RestingHr { get; set; }
    public int? MaxHr { get; set; }
    public string TimeZone { get; set; } = "Europe/Copenhagen";
}