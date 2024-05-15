using EmBackend.Entities;
using EmBackend.Models.Movements;
using EmBackend.Models.Users.Requests;
using EmBackend.Utilities.Validators;
using FluentValidation;

namespace EmBackend.Utilities;

public class Validation
{
    public readonly IValidator<MovementDto> MovementDtoValidator;
    public readonly IValidator<Movement> MovementValidator;
    public readonly IValidator<Category> CategoryValidator;
    public readonly IValidator<User> UserValidator;
    public readonly IValidator<UpdateUserRequest> UpdateUserValidator;
    
    public Validation()
    {
        MovementDtoValidator = new MovementDtoValidator();
        MovementValidator = new MovementValidator();
        CategoryValidator = new CategoryValidator();
        UserValidator = new UserValidator();
        UpdateUserValidator = new UserUpdateValidator();
    }
}