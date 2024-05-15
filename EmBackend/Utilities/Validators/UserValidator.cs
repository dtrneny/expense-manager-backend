using EmBackend.Entities;
using FluentValidation;

namespace EmBackend.Utilities.Validators;

public class UserValidator: AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Firstname)
            .NotEmpty()
            .MaximumLength(15);
        
        RuleFor(user => user.Lastname)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(user => user.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(user => user.Password)
            .NotEmpty();
    }
}