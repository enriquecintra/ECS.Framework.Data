using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace ECS.Framework.Data
{
    public class CacheWrapper
    {
        private IConfiguration _config;
        private ConnectionMultiplexer _connection;
        public CacheWrapper(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void Init()
        {

            var _redisHost = _config["Redis:Host"];
            var _redisPort = Convert.ToInt32(_config["Redis:Port"]);
            var configString = $"{_redisHost}:{_redisPort},connectRetry=5";

            _connection = ConnectionMultiplexer.Connect(configString);
        }

        public void Set<T>(string key, T value)
        {

            var db = _connection.GetDatabase();
            db.StringSet(key, JsonConvert.SerializeObject(value), null, When.NotExists, CommandFlags.HighPriority);
        }

        public T Get<T>(string key)
        {
            var db = _connection.GetDatabase();
            return JsonConvert.DeserializeObject<T>(db.StringGet(key));
        }   

        public void Set(string key, string value)
        {
            var db = _connection.GetDatabase();
            db.StringSet(key, JsonConvert.SerializeObject(value));
        }

        public string Get(string key)
        {
            var db = _connection.GetDatabase();
            return db.StringGet(key);
        }

        public bool Exists(string key)
        {
            var db = _connection.GetDatabase(); 
            return db.KeyExists(key);
        }


    }
}
