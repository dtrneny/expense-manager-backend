using EmBackend.Models.Helpers;

namespace EmBackend.Models.Movements.Params;

public record GetMovementsParams (
    FilterParam<string[]>? CategoryIds,
    List<FilterParam<DateTime>>? Timestamp
);
