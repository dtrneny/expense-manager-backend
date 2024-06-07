using EmBackend.Models.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EmBackend.Utilities;

public static class MongoDbDefinitionBuilder
{
    public static FilterDefinition<T>? BuildFilterDefinition<T>(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>?> filterAction)
    {
        var filterBuilder = Builders<T>.Filter;

        return filterBuilder != null
            ? filterAction(filterBuilder)
            : null;
    }
    
    public static UpdateDefinition<T>? BuildUpdateDefinition<T>(BsonDocument? changesDocument)
    {
        if (changesDocument == null) { return null; }
        
        var builder = Builders<T>.Update;
        if (builder == null) { return null; }
            
        UpdateDefinition<T>? update = null;
        foreach (var change in changesDocument)
        {
            if (update == null)
            {
                update = builder.Set(change.Name, change.Value);
            }
            else
            {
                update = update.Set(change.Name, change.Value);
            }
        }

        return update;
    }
    
    public static FilterDefinition<T>? BuildFilterDefinitionFromQuery<T, TParams>(TParams queryParams)
    {
        var builder = Builders<T>.Filter;
        if (builder == null) { return null; }
        
        var filter = builder.Empty;

        var objectType = typeof(T);
        
        foreach (var property in typeof(TParams).GetProperties())
        {
            if (objectType.GetProperty(property.Name) == null) { continue; }

            var propertyValue = property.GetValue(queryParams);
            if (propertyValue == null) { continue; }

            var propertyType = propertyValue.GetType();
                
            if (!IsPropertyList(propertyType))
            {
                if (!IsPropertyFilterParam(propertyType)) { continue; }

                var operatorValue = propertyType.GetProperty("Operator")?.GetValue(propertyValue);
                var value = propertyType.GetProperty("Value")?.GetValue(propertyValue);  
                if (operatorValue == null || value == null) { continue; }
                
                if (operatorValue.GetType() != typeof(FilterOperator)) { continue; }
                
                var filterExtension = AssembleFilterCondition(builder, property.Name, (FilterOperator)operatorValue, value);
                if (filterExtension == null) { return null; }
                
                filter &= filterExtension;
            }
            else
            {
                var propertyList = (IEnumerable<object>)propertyValue;
                
                foreach (var subFilterObject in propertyList)
                {
                    var subFilterType = subFilterObject.GetType();
                    if (!IsPropertyFilterParam(subFilterType)) { continue; }
                    var operatorValue = subFilterType.GetProperty("Operator")?.GetValue(subFilterObject);
                    var value = subFilterType.GetProperty("Value")?.GetValue(subFilterObject);  
                    if (operatorValue == null || value == null) { continue; }
                    
                    if (operatorValue.GetType() != typeof(FilterOperator)) { continue; }
                
                    var filterExtension = AssembleFilterCondition(builder, property.Name, (FilterOperator)operatorValue, value);
                    if (filterExtension == null) { return null; }
                
                    filter &= filterExtension;
                }
            }
        }
        
        return filter;
    }
    
    private static bool IsPropertyList(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>);
    }
    
    private static bool IsPropertyFilterParam(Type propertyType)
    {
        return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(FilterParam<>);
    }

    private static FilterDefinition<T>? AssembleFilterCondition<T>(
        FilterDefinitionBuilder<T> builder,
        string fieldName,
        FilterOperator operatorValue,
        object filterValue
    )
    {
        if (builder == null) { return null; }

        return operatorValue switch
        {
            FilterOperator.In => builder.In(fieldName, (IEnumerable<object>)filterValue),
            FilterOperator.Eq => builder.Eq(fieldName, filterValue),
            FilterOperator.Gt => builder.Gt(fieldName, filterValue),
            FilterOperator.Lt => builder.Lt(fieldName, filterValue),
            _ => null
        };
    }
}