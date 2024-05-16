namespace EmBackend.Models.Movements.Requests;

public record PostMovementRequest (
    int Amount,
    string Label,
    string UserId,
    DateTime Timestamp,
    List<string> CategoryIds
);
