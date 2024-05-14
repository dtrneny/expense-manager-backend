namespace EmBackend.Models.Movements;

public record MovementDto (
    string Id,
    string UserId,
    double Amount,
    string Label,
    List<string> CategoryIds
);
    