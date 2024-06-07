using FluentValidation;
using MongoDB.Bson;

namespace EmBackend.Validation.Rules;

public static class ObjectIdRule
{
    public static IRuleBuilderOptions<T, string> IsObjectId<T>(this IRuleBuilder<T, string> ruleBuilder) {
        return ruleBuilder
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("Value is not a valid object id.");
    }
}