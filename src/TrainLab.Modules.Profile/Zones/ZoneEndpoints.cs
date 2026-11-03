using System.Security.Claims;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

using TrainLab.Modules.Profile.Persistence;
using TrainLab.SharedKernel.Users;

namespace TrainLab.Modules.Profile.Zones;

public static class ZoneEndpoints
{
    public static IEndpointRouteBuilder MapZoneEndpoints(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/zones").WithTags("Zones");

        group.MapGet("/", GetZones);
        group.MapPost("/", CreateZone);
        group.MapGet("/effective", GetEffectiveZone);

        return api;
    }

    private static async Task<Ok<List<ZoneSetResponse>>> GetZones(
        ClaimsPrincipal user,
        ProfileDbContext db,
        CancellationToken ct)
    {
        var userId = user.GetUserId();

        var zoneSets = await db.ZoneSets
            .Where(z => z.UserId == userId)
            .OrderBy(z => z.Sport)
            .ThenBy(z => z.EffectiveFrom)
            .ToListAsync(ct);

        return TypedResults.Ok(zoneSets.Select(ZoneSetResponse.From).ToList());
    }

    private static async Task<Results<Created<ZoneSetResponse>, ValidationProblem, Conflict>> CreateZone(
        CreateZoneSetRequest request,
        IValidator<CreateZoneSetRequest> validator,
        ClaimsPrincipal user,
        ProfileDbContext db,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var userId = user.GetUserId();

        var alreadyExists = await db.ZoneSets.AnyAsync(z =>
            z.UserId == userId &&
            z.Sport == request.Sport &&
            z.EffectiveFrom == request.EffectiveFrom, ct);

        if (alreadyExists)
        {
            return TypedResults.Conflict();
        }

        var zoneSet = new ZoneSet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Sport = request.Sport,
            Lthr = request.Lthr,
            EffectiveFrom = request.EffectiveFrom,
        };

        db.ZoneSets.Add(zoneSet);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/zones/{zoneSet.Id}", ZoneSetResponse.From(zoneSet));
    }

    private static async Task<Results<Ok<ZoneSetResponse>, NotFound>> GetEffectiveZone(
        Sport sport,
        DateOnly date,
        ZoneSetLookup lookup,
        ClaimsPrincipal user,
        CancellationToken ct)
    {
        var zoneSet = await lookup.FindEffectiveAsync(user.GetUserId(), sport, date, ct);

        return zoneSet is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(ZoneSetResponse.From(zoneSet));
    }
}