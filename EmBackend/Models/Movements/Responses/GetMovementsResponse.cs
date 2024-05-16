namespace EmBackend.Models.Movements.Responses;

public record GetMovementsResponse (
    List<MovementDto> Movements
);
