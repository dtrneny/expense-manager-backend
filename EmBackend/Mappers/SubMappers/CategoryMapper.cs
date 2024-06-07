using EmBackend.Entities;
using EmBackend.Models.Categories;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Mappers.SubMappers;

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryDto MapCategoryToCategoryDto(Category entity);
}