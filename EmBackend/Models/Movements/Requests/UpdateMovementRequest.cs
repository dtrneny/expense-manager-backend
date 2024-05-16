namespace EmBackend.Models.Movements.Requests;

public record UpdateMovementRequest (
    double? Amount,
    string? Label,
    DateTime? Timestamp,
    List<string> CategoryIds
);
