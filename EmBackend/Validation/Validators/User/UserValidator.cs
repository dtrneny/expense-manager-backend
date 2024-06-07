using FluentValidation;

namespace EmBackend.Validation.Validators.User;

public class UserValidator: AbstractValidator<Entities.User>
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