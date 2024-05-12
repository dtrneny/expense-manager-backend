using EmBackend.Models.Movements;
using FluentValidation;

namespace EmBackend.Utilities;

public class Validation
{
    public readonly IValidator<MovementDto> MovementDtoValidator;
    
    public Validation(
        IValidator<MovementDto> movementDtoValidator
    )
    {
        MovementDtoValidator = movementDtoValidator;
    }
}