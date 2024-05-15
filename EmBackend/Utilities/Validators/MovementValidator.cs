using EmBackend.Entities;
using FluentValidation;

namespace EmBackend.Utilities.Validators;

public class MovementValidator: AbstractValidator<Movement>
{
    public MovementValidator()
    {
        RuleFor(movement => movement.UserId)
            .NotNull()
            .NotEmpty();
        
        RuleFor(movement => movement.Amount)
            .NotNull();
        
        RuleFor(movement => movement.Label)
            .NotEmpty()
            .MaximumLength(15);
    }
}