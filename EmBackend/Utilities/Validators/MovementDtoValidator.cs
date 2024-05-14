using EmBackend.Models.Movements;
using FluentValidation;

namespace EmBackend.Utilities.Validators;

public class MovementDtoValidator : AbstractValidator<MovementDto>
{
    public MovementDtoValidator()
    {
        RuleFor(dto => dto.Amount)
            .NotEmpty();

        RuleFor(dto => dto.UserId)!
            .NotEmpty();
    }
}