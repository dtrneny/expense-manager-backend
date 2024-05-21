namespace EmBackend.Models.Movements;

public record MovementDto (
    string Id,
    string UserId,
    double Amount,
    string Label,
    DateTime Timestamp,
    List<string> CategoryIds
);
    