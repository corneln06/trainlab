namespace TrainLab.Modules.Profile.Zones;

public record ZoneSetResponse(
    Guid Id,
    Sport Sport,
    int Lthr,
    DateOnly EffectiveFrom)
{

    public static ZoneSetResponse From(ZoneSet zoneSet) => new ZoneSetResponse(
        zoneSet.Id,
        zoneSet.Sport,
        zoneSet.Lthr,
        zoneSet.EffectiveFrom);
}

public record CreateZoneSetRequest(
    Sport Sport,
    int Lthr,
    DateOnly EffectiveFrom);

