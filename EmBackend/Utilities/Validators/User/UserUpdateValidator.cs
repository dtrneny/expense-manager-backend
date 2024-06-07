using EmBackend.Models.Users.Requests;
using FluentValidation;

namespace EmBackend.Utilities.Validators.User;

public class UserUpdateValidator: AbstractValidator<UpdateUserRequest>
{
    public UserUpdateValidator()
    {
        When(update => update.Firstname != null, () =>
        {
            RuleFor(update => update.Firstname)
                .NotEmpty()
                .MaximumLength(15);
        });

        When(update => update.Lastname != null, () =>
        {
            RuleFor(update => update.Lastname)
                .NotEmpty()
                .MaximumLength(15);
        });

        When(update => update.Email != null, () =>
        {
            RuleFor(update => update.Email)
                .NotEmpty()
                .EmailAddress();
        });
    }
}