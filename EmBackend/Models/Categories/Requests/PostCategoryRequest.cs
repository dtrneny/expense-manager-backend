using EmBackend.Entities.Helpers;

namespace EmBackend.Models.Categories.Requests;

public record PostCategoryRequest (
    string Name,
    CategoryOwnership Ownership,
    string? OwnerId
);
