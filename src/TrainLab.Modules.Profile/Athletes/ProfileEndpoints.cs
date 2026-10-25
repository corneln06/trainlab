using System.Security.Claims;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

using TrainLab.Modules.Profile.Persistence;
using TrainLab.SharedKernel.Users;

namespace TrainLab.Modules.Profile.Athletes;

public static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder api)
    {
        var group = api.MapGroup("/profile");

        group.MapGet("/", GetProfile);
        group.MapPut("/", UpdateProfile);
        return api;
    }

    private static async Task<Ok<AthleteProfileResponse>> GetProfile(
        ClaimsPrincipal user, ProfileDbContext db, CancellationToken ct)
    {
        var profile =
            await db.AthleteProfiles.FindAsync([user.GetUserId()], ct);
        var response = profile is null
            ? AthleteProfileResponse.Default()
            : AthleteProfileResponse.From(profile);

        return TypedResults.Ok(response);
    }

    private static async
        Task<Results<Ok<AthleteProfileResponse>, ValidationProblem>>
        UpdateProfile(UpdateAthleteProfileRequest request, ProfileDbContext db, ClaimsPrincipal user, CancellationToken ct, IValidator<UpdateAthleteProfileRequest> requestValidator)
    {
        var validation = await requestValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return TypedResults.ValidationProblem(validation.ToDictionary());
        }

        var userId = user.GetUserId();
        var profile = await db.AthleteProfiles.FindAsync([userId], ct);
        if (profile is null)
        {
            profile = new AthleteProfile() { UserId = userId };
            db.AthleteProfiles.Add(profile);
        }
        profile.Sex = request.Sex;
        profile.BirthYear = request.BirthYear;
        profile.WeightKg = request.WeightKg;
        profile.RestingHr = request.RestingHr;
        profile.MaxHr = request.MaxHr;
        profile.TimeZone = request.TimeZone;

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(AthleteProfileResponse.From(profile));
    }
}