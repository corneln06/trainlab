namespace TrainLab.Modules.Profile.Zones;

public class ZoneSet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Sport Sport { get; set; }
    public int Lthr { get; set; }
    public DateOnly EffectiveFrom { get; set; }
}