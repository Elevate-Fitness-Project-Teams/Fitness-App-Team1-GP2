using FluentValidation;

namespace WorkoutService.Feature.Exercises.GetExerciseById;

public sealed class GetExerciseByIdValidator : AbstractValidator<GetExerciseByIdQuery>
{
    public GetExerciseByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Exercise id must be greater than 0.");
    }
}