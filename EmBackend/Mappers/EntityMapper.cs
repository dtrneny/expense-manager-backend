using EmBackend.Mappers.SubMappers;

namespace EmBackend.Mappers;

public class EntityMapper
{
    public readonly CategoryMapper CategoryMapper;
    public readonly MovementMapper MovementMapper;
    public readonly UserMapper UserMapper;

    public EntityMapper()
    {
        CategoryMapper = new CategoryMapper();
        MovementMapper = new MovementMapper();
        UserMapper = new UserMapper();
    }
}