using EmBackend.Validation.Rules;
using FluentValidation;

namespace EmBackend.Validation.Validators.Users;

public class UserValidator: AbstractValidator<Entities.User>
{
    public UserValidator()
    {
        When(user => user.Id != null, () =>
        {
            RuleFor(user => user.Id!)
                .IsObjectId();
        });
        
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