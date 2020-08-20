using LeoMadeiras.Framework.Configuration;

namespace ECS.Framework.Data.RethinkDB
{
    /// <summary>
    /// Configuracões de conexão do RethinkDb
    /// </summary>
    public class RethinkDBOptions
    {
        /// <summary>
        /// Host (dns name, ip) para conexão ao redis
        /// </summary>
        public string Host { get; set; } = EnvironmentHelper.GetValueFromEnv<string>("RETHINKDB_HOST");

        /// <summary>
        /// Porta
        /// </summary>
        public int Port { get; set; } = EnvironmentHelper.GetValueFromEnv<int>("RETHINKDB_PORT");

        /// <summary>
        /// Nome do banco de dados
        /// </summary>
        public string Database { get; set; } = EnvironmentHelper.GetValueFromEnv<string>("RETHINKDB_DATABASE");

        /// <summary>
        /// Valor default de timeout durante as operações
        /// </summary>
        public int Timeout { get; set; } = EnvironmentHelper.GetValueFromEnv<int>("RETHINKDB_TIMEOUT");

        public string Username { get; set; } = EnvironmentHelper.GetValueFromEnv<string>("RETHINKDB_USERNAME", throwException: false);
        public string Password { get; set; } = EnvironmentHelper.GetValueFromEnv<string>("RETHINKDB_PASSWORD", throwException: false);
        public bool EnableSsl { get; set; } = EnvironmentHelper.GetValueFromEnv<bool>("RETHINKDB_SSL", throwException: false);
    }
}