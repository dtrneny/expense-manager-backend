using EmBackend.Entities;
using EmBackend.Models.Movements;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Mappers.SubMappers;

[Mapper]
public partial class MovementMapper
{
    public partial MovementDto MapMovementToMovementDto(Movement entity);
}