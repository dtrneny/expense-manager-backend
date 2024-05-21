namespace EmBackend.Models.Helpers;

public record FilterParam<T>(
    FilterOperator Operator,
    T Value
);