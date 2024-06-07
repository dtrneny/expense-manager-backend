using EmBackend.Models.Movements.Requests;
using FluentValidation;

namespace EmBackend.Validation.Validators.Movement;

public class MovementUpdateValidator: AbstractValidator<UpdateMovementRequest>
{
    public MovementUpdateValidator()
    {
        RuleFor(movement => movement.Amount)
            .NotNull();
        
        RuleFor(movement => movement.Label)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(movement => movement.Timestamp)
            .NotNull();
    }
}
