using EmBackend.Entities;
using EmBackend.Models.Categories;
using EmBackend.Models.Categories.Requests;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Utilities.Mappers;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryDto MapCategoryToCategoryDto(Category entity);
}