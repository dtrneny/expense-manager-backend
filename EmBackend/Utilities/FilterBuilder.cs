using MongoDB.Driver;

namespace EmBackend.Utilities;

public class FilterBuilder<T>
{
    public FilterDefinition<T>? BuildFilterDefinition(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>?> filterAction)
    {
        var filterBuilder = Builders<T>.Filter;

        if (filterBuilder == null) { return null; }
        
        return filterAction(filterBuilder);
    }
}