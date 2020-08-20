using Microsoft.Extensions.DependencyInjection;

namespace ECS.Framework.Data.RethinkDB
{
    public static class RethinkDBExtensions
    {
        public static IServiceCollection AddRethinkDb(this IServiceCollection services)
        {
            services.AddSingleton<RethinkDBOptions>();
            services.AddSingleton<IRethinkDBConnectionFactory, RethinkDBConnectionFactory>();

            return services;
        }
    }
}