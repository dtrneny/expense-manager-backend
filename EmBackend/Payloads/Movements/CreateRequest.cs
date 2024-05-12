namespace EmBackend.Payloads.Movements;

public record CreateRequest (
    int Amount,
    string Label
);