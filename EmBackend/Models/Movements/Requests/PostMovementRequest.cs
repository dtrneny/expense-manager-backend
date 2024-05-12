namespace EmBackend.Models.Movements.Requests;

public record PostMovementRequest (
    int Amount,
    string Label
);
