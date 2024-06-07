namespace EmBackend.Models.Categories.Responses;

public record GetCategoriesResponse (
    List<CategoryDto> Categories
);
