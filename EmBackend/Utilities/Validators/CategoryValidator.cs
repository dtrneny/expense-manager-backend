using EmBackend.Entities;
using EmBackend.Entities.Helpers;
using FluentValidation;

namespace EmBackend.Utilities.Validators;

public class CategoryValidator: AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(category => category.Name)
            .NotEmpty()
            .MaximumLength(15);
        
        RuleFor(category => category.Ownership)
            .IsInEnum();

        When(category => category.Ownership == CategoryOwnership.User, () =>
        {
            RuleFor(category => category.OwnerId)
                .NotNull();
        });
        
        When(category => category.Ownership == CategoryOwnership.Default, () =>
        {
            RuleFor(category => category.OwnerId)
                .Null();
        });
    }
}