using EmBackend.Entities.Helpers;
using EmBackend.Validation.Rules;
using FluentValidation;

namespace EmBackend.Validation.Validators.Categories;

public class CategoryValidator: AbstractValidator<Entities.Category>
{
    public CategoryValidator()
    {
        When(category => category.Id != null, () =>
        {
            RuleFor(category => category.Id!)
                .IsObjectId();
        });
        
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