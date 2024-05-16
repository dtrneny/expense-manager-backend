using EmBackend.Entities;
using EmBackend.Models.Categories.Requests;
using EmBackend.Models.Movements;
using EmBackend.Models.Movements.Requests;
using EmBackend.Models.Users.Requests;
using EmBackend.Utilities.Validators;
using EmBackend.Utilities.Validators.Category;
using EmBackend.Utilities.Validators.Movement;
using EmBackend.Utilities.Validators.User;
using EmBackend.Utilities.Validators.UserValidators;
using FluentValidation;

namespace EmBackend.Utilities;

public class Validation
{
    public readonly IValidator<Movement> MovementValidator;
    public readonly IValidator<Category> CategoryValidator;
    public readonly IValidator<User> UserValidator;
    public readonly IValidator<UpdateUserRequest> UpdateUserValidator;
    public readonly IValidator<UpdateMovementRequest> UpdateMovementValidator;
    public readonly IValidator<UpdateCategoryRequest> UpdateCategoryValidator;
    
    public Validation()
    {
        MovementValidator = new MovementValidator();
        CategoryValidator = new CategoryValidator();
        UserValidator = new UserValidator();
        UpdateUserValidator = new UserUpdateValidator();
        UpdateMovementValidator = new MovementUpdateValidator();
        UpdateCategoryValidator = new CategoryUpdateValidator();
    }
}