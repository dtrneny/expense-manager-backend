using EmBackend.Validation.Rules;
using FluentValidation;

namespace EmBackend.Validation.Validators;

public class ObjectIdValidator: AbstractValidator<string>
{
    public ObjectIdValidator()
    {
        RuleFor(id => id)
            .IsObjectId();
    }
}