using MongoDB.Bson;
using MongoDB.Driver;

namespace EmBackend.Utilities;

public static class EntityOperationBuilder<T>
{
    public static FilterDefinition<T>? BuildFilterDefinition(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>?> filterAction)
    {
        var filterBuilder = Builders<T>.Filter;

        return filterBuilder != null
            ? filterAction(filterBuilder)
            : null;
    }
    
    public static UpdateDefinition<T>? BuildUpdateDefinition(BsonDocument? changesDocument)
    {
        if (changesDocument == null) { return null; }
        
        var builder = Builders<T>.Update;
        if (builder == null) { return null; }
            
        UpdateDefinition<T>? update = null;
        foreach (var change in changesDocument)
        {
            if (update == null) { update = builder.Set(change.Name, change.Value); }
            else { update = update.Set(change.Name, change.Value); }
        }

        return update;
    }
}