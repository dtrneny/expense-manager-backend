namespace EmBackend.Models.Movements.Requests;

public record UpdateMovementRequest (
    double Amount,
    string Label,
    List<string> CategoryIds
);
