using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ECS.Framework.Data.MongoDB
{
    public static class MongoDBExtensions
    {
        public static IServiceCollection AddMongoDB(this IServiceCollection services)
        {
            var mongoUri = Configuration.EnvironmentHelper.GetValueFromEnv<string>("MONGO_URI");

            // MongoClient (Singleton)
            var mongoUrl = new MongoUrl(mongoUri);
            var mongoConnection = new MongoDBConnectionWraper
            {
                MongoURL = mongoUrl,
                MongoClient = new MongoClient(mongoUrl)
            };

            services.AddSingleton(mongoConnection);

            services.AddHealthChecks()
                .AddMongoDb(mongoUri, "mongodb", failureStatus: null, tags: new string[] { "db", "mongodb" }, null);

            return services;
        }
    }
}