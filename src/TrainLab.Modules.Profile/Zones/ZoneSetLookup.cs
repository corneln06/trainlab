using Microsoft.EntityFrameworkCore;

using TrainLab.Modules.Profile.Persistence;

namespace TrainLab.Modules.Profile.Zones;

public class ZoneSetLookup(ProfileDbContext db)
{
    public async Task<ZoneSet?> FindEffectiveAsync(Guid userId, Sport sport,
        DateOnly date, CancellationToken ct)
    {

        var zoneSet = await db.ZoneSets.Where(p =>
                p.UserId == userId && p.Sport == sport &&
                p.EffectiveFrom <= date)
            .OrderByDescending(p => p.EffectiveFrom).FirstOrDefaultAsync(ct);

        return zoneSet;
    }
}