using RethinkDb.Driver.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ECS.Framework.Data.RethinkDB
{
    /// <summary>
    /// Factory para instanciação da conexão do rethinkDb
    /// </summary>
    public class RethinkDBConnectionFactory : IRethinkDBConnectionFactory
    {
        // R singleton
        private static RethinkDb.Driver.RethinkDB R = RethinkDb.Driver.RethinkDB.R;

        // conexão atual
        private Connection _conn;

        /// <summary>
        /// Configurações de conexão que está sendo utilizada.
        /// </summary>
        public RethinkDBOptions Options { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options_">configurações do rethinkDb</param>
        public RethinkDBConnectionFactory(RethinkDBOptions options_)
        {
            this.Options = options_;
        }

        /// <summary>
        /// Conecta ao banco de dados especificado
        /// </summary>
        /// <returns>Conexão</returns>
        public async Task<Connection> ConnectAsync()
        {
            if (_conn == null)
            {
                //var sslContext = new SslContext()
                //{
                //    EnabledProtocols = System.Security.Authentication.SslProtocols.Tls12,
                //    ServerCertificateValidationCallback = ValidationCallback,
                //};
                //.EnableSsl(sslContext, string.Empty, string.Empty)

                var _config = R.Connection()
                 .Hostname(Options.Host)
                 .Port(Options.Port)
                 .Timeout(Options.Timeout);

                if (!string.IsNullOrEmpty(Options.Username))
                    _config = _config.User(Options.Username, Options.Password);

                _conn = await _config.ConnectAsync();
            }

            if (!_conn.Open)
                _conn.Reconnect();

            return _conn;
        }

        private bool ValidationCallback(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Disconecta do banco de dados
        /// </summary>
        public void Disconnect()
        {
            if (_conn != null && _conn.Open)
                _conn.Close(shouldNoReplyWait: false);
        }
    }

    /// <summary>
    /// Factory para instanciação da conexão do rethinkDb
    /// </summary>
    public interface IRethinkDBConnectionFactory
    {
        /// <summary>
        /// Configurações de conexão que está sendo utilizada.
        /// </summary>
        RethinkDBOptions Options { get; }

        /// <summary>
        /// Conecta ao banco de dados especificado
        /// </summary>
        /// <returns>Conexão</returns>
        Task<Connection> ConnectAsync();

        /// <summary>
        /// Disconecta do banco de dados
        /// </summary>
        void Disconnect();
    }
}