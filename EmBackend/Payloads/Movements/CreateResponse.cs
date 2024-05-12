using EmBackend.DTOs;

namespace EmBackend.Payloads.Movements;

public record CreateResponse (
    MovementDto Movement
);