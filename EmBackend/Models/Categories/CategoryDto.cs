using EmBackend.Entities.Helpers;

namespace EmBackend.Models.Categories;

public record CategoryDto (
    string Id,
    string Name,
    CategoryOwnership Ownership,
    string? OwnerId
);
