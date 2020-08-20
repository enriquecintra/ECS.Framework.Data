using MongoDB.Driver;

namespace ECS.Framework.Data.MongoDB
{
    public class MongoDBConnectionWraper
    {
        public MongoClient MongoClient { get; set; }

        public MongoUrl MongoURL { get; set; }
    }
}