namespace TrainLab.Modules.Profile.Athletes;

using FluentValidation;

public class UpdateAthleteProfileValidator : AbstractValidator<UpdateAthleteProfileRequest>
{
    public UpdateAthleteProfileValidator(TimeProvider timeProvider)
    {
        var currentYear = timeProvider.GetUtcNow().Year;

        RuleFor(x => x.RestingHr).InclusiveBetween(30, 100);
        RuleFor(x => x.MaxHr).InclusiveBetween(120, 230);
        RuleFor(x => x.RestingHr).LessThan(x => x.MaxHr).When(x => x.RestingHr.HasValue && x.MaxHr.HasValue).WithMessage("Resting heart rate must be lower than max heart rate.");
        RuleFor(x => x.WeightKg).InclusiveBetween(30, 200);
        RuleFor(x => x.BirthYear).InclusiveBetween(1900, currentYear);
        RuleFor(x => x.TimeZone).NotEmpty().Must(timeZone => TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out _))
            .WithMessage("Unknown time zone.");
    }
}