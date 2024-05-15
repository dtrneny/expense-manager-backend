using EmBackend.Entities;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Utilities.Mappers;

[Mapper(AllowNullPropertyAssignment = false)]
public partial class CategoryUpdateMapper
{
    public partial Category ApplyUpdate(Category entity);
}