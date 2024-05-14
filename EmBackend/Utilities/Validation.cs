using EmBackend.Entities;
using EmBackend.Models.Movements;
using EmBackend.Utilities.Validators;
using FluentValidation;

namespace EmBackend.Utilities;

public class Validation
{
    public readonly IValidator<MovementDto> MovementDtoValidator;
    public readonly IValidator<Category> CategoryValidator;
    
    public Validation()
    {
        MovementDtoValidator = new MovementDtoValidator();
        CategoryValidator = new CategoryValidator();
    }
}