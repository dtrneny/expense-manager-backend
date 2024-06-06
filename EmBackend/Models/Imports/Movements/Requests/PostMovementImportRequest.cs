namespace EmBackend.Models.Imports.Movements.Requests;

public record PostMovementImportRequest (
    List<ImportMovementDto> ImportedMovements
);