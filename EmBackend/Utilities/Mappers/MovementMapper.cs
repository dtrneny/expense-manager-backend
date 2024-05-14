using EmBackend.Entities;
using EmBackend.Models.Movements;
using EmBackend.Models.Movements.Requests;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Utilities.Mappers;

[Mapper]
public partial class MovementMapper
{
    public partial MovementDto MapMovementToMovementDto(Movement entity);
}