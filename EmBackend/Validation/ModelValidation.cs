using EmBackend.Entities;
using EmBackend.Models.Categories.Requests;
using EmBackend.Models.Movements.Requests;
using EmBackend.Models.Users.Requests;
using EmBackend.Validation.Validators;
using EmBackend.Validation.Validators.Categories;
using EmBackend.Validation.Validators.Movements;
using EmBackend.Validation.Validators.Users;
using FluentValidation;

namespace EmBackend.Validation;

public class ModelValidation
{
    public readonly IValidator<Movement> MovementValidator;
    public readonly IValidator<Category> CategoryValidator;
    public readonly IValidator<User> UserValidator;
    public readonly IValidator<UpdateUserRequest> UpdateUserValidator;
    public readonly IValidator<UpdateMovementRequest> UpdateMovementValidator;
    public readonly IValidator<UpdateCategoryRequest> UpdateCategoryValidator;
    public readonly ObjectIdValidator ObjectIdValidator;
    
    public ModelValidation()
    {
        MovementValidator = new MovementValidator();
        CategoryValidator = new CategoryValidator();
        UserValidator = new UserValidator();
        UpdateUserValidator = new UserUpdateValidator();
        UpdateMovementValidator = new MovementUpdateValidator();
        UpdateCategoryValidator = new CategoryUpdateValidator();
        ObjectIdValidator = new ObjectIdValidator();
    }
}