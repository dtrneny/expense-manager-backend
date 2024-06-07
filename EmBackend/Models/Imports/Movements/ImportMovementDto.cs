namespace EmBackend.Models.Imports.Movements;

public record ImportMovementDto (
    double Amount,
    string Label,
    DateTime Timestamp,
    List<string> CategoryNames
);