using EmBackend.Entities.Helpers;

namespace EmBackend.Models.Categories.Requests;

public record UpdateCategoryRequest (
    string? Name,
    CategoryOwnership? Ownership,
    string? OwnerId
);
