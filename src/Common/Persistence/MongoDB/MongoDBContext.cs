using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Common.Persistence.MongoDB;

public static class MongoDBContext
{
    public static IMongoDatabase Connect(MongoDBConfig configs)
    {
        Configure();

        var client = new MongoClient(configs.ConnectionString);
        return client.GetDatabase(configs.DatabaseName);
    }

    private static void Configure()
    {
        // Set up enum to string convertor (applies to all entities)
        ConventionRegistry.Register("EnumStringConvention",
            new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, x => true);
    }
}