using FluentValidation;

namespace TrainLab.Modules.Profile.Zones;

public class CreateZoneSetValidator : AbstractValidator<CreateZoneSetRequest>
{
    public CreateZoneSetValidator(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        RuleFor(x => x.Sport).IsInEnum();

        RuleFor(x => x.Lthr).InclusiveBetween(100, 220);

        RuleFor(x => x.EffectiveFrom)
            .LessThanOrEqualTo(today)
            .WithMessage("Zones can't start in the future.");
    }
}