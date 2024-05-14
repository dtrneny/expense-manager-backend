using EmBackend.Utilities.Mappers;

namespace EmBackend.Utilities;

public class EntityMapper
{
    public readonly CategoryMapper CategoryMapper;
    public readonly MovementMapper MovementMapper;

    public EntityMapper()
    {
        CategoryMapper = new CategoryMapper();
        MovementMapper = new MovementMapper();
    }
}