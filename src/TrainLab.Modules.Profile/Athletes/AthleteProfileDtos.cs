namespace TrainLab.Modules.Profile.Athletes;

public record AthleteProfileResponse(
    Sex? Sex,
    int? BirthYear,
    double? WeightKg,
    int? RestingHr,
    int? MaxHr,
    string TimeZone)
{
    public static AthleteProfileResponse From(AthleteProfile profile) => new(
        profile.Sex,
        profile.BirthYear,
        profile.WeightKg,
        profile.RestingHr,
        profile.MaxHr,
        profile.TimeZone);

    public static AthleteProfileResponse Default() => From(new AthleteProfile());
}

public record UpdateAthleteProfileRequest(
    Sex Sex,
    int? BirthYear,
    double? WeightKg,
    int? RestingHr,
    int? MaxHr,
    string TimeZone);