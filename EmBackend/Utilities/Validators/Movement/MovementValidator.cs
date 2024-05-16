using FluentValidation;

namespace EmBackend.Utilities.Validators.Movement;

public class MovementValidator: AbstractValidator<Entities.Movement>
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
        
        RuleFor(movement => movement.Timestamp)
            .NotNull();
    }
}