using EmBackend.Entities.Helpers;
using EmBackend.Models.Categories.Requests;
using FluentValidation;

namespace EmBackend.Validation.Validators.Categories;

public class CategoryUpdateValidator: AbstractValidator<UpdateCategoryRequest>
{
    public CategoryUpdateValidator()
    {
        RuleFor(update => update.Name)
            .NotEmpty()
            .MaximumLength(15);
        
        RuleFor(update => update.Ownership)
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
