using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace ECS.Framework.Data
{
    public class DapperRepositoryUtils
    {
        private IConfiguration _config;
        public DapperRepositoryUtils(IConfiguration configuration)
        {
            _config = configuration;
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_config.GetConnectionString("Connection"));
            connection.Open();
            return connection;
        }
    }
}
