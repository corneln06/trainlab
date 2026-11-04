namespace TrainLab.Modules.Profile.Zones;

public record ZoneSetResponse(
    Guid Id,
    Sport Sport,
    int Lthr,
    DateOnly EffectiveFrom,
    IReadOnlyList<HeartRateZone> Zones)
{

    public static ZoneSetResponse From(ZoneSet zoneSet) => new ZoneSetResponse(
        zoneSet.Id,
        zoneSet.Sport,
        zoneSet.Lthr,
        zoneSet.EffectiveFrom,
        ZoneCalculator.Calculate(zoneSet.Sport, zoneSet.Lthr));
}

public record CreateZoneSetRequest(
    Sport Sport,
    int Lthr,
    DateOnly EffectiveFrom);