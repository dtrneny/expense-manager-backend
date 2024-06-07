using MongoDB.Bson;

namespace EmBackend.Utilities;

public static class BsonUtility
{
    public static BsonDocument ToBsonDocument<T>(T record)
    {
        var bsonDocument = new BsonDocument();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var name = property.Name;
            var value = property.GetValue(record);

            if (value is not null) { bsonDocument.Add(name, BsonValue.Create(value)); }
        }

        return bsonDocument;
    }
}