namespace EmBackend.Models.Exports.Movements;

public record ExportMovementDto(
    string Id,
    double Amount,
    string Label,
    DateTime Timestamp,
    List<string> CategoryNames
);
