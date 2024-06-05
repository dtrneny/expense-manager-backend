using EmBackend.Entities;
using EmBackend.Utilities.Enums;

namespace EmBackend.Services;

public class StatisticsService
{
    public string? GetKeyMovementCategoryId(IEnumerable<Movement> movements, MovementFilterType? type = MovementFilterType.All)
    {
        var keyMovementCategoryId = movements
            .ToList()
            .Where(movement =>
            {
                return type switch
                {
                    MovementFilterType.Income => movement.Amount > 0,
                    MovementFilterType.Expense => movement.Amount < 0,
                    MovementFilterType.All => true,
                    _ => false
                };
            })
            .SelectMany(movement => movement.CategoryIds)
            .GroupBy(id => id)
            .Select(group => new { CategoryId = group.Key, Count = group.Count() })
            .MaxBy(group => group.Count);

        return keyMovementCategoryId?.CategoryId;
    }
    
    public double GetMovementSum(IEnumerable<Movement> movements, MovementFilterType? type = MovementFilterType.All)
    {
        var sum = movements
            .ToList()
            .Where(movement =>
            {
                return type switch
                {
                    MovementFilterType.Income => movement.Amount > 0,
                    MovementFilterType.Expense => movement.Amount < 0,
                    MovementFilterType.All => true,
                    _ => false
                };
            })
            .Sum(movement => movement.Amount);

        return sum;
    }
    
}