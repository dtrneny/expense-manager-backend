using EmBackend.Entities;
using EmBackend.Models.Movements;
using Riok.Mapperly.Abstractions;

namespace EmBackend.Utilities.Mappers;

[Mapper]
public partial class MovementMapper
{
    public partial MovementDto MovementToMovementDto(Movement movement);
}