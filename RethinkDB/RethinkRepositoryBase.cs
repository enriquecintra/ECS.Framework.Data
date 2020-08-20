using RethinkDb.Driver.Net;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECS.Framework.Data.RethinkDB
{
    public abstract class RethinkRepositoryBase<TEntity> : IRethinkRepository<TEntity>
        where TEntity : RethinkEntityBase
    {
        public Task<Connection> Instance() => _factory.ConnectAsync();

        protected readonly IRethinkDBConnectionFactory _factory;

        protected static RethinkDb.Driver.RethinkDB R = RethinkDb.Driver.RethinkDB.R;

        protected string _dbName;

        public RethinkRepositoryBase(IRethinkDBConnectionFactory factory_)
        {
            this._factory = factory_;
            this._dbName = _factory.Options.Database;
        }

        /// <summary>
        /// Método principal de inicialização do banco de dados.
        /// </summary>
        /// <returns>awaitable</returns>
        public virtual async Task InitializeDatabaseAsync()
        {
            // database
            await CreateDbAsync(_dbName);

            await InitializeTablesAsync();
            await CreateIndexAsync(_dbName, GetTableName(), nameof(RethinkEntityBase.Id));
            await InitializeIndexAsync();
        }

        /// <summary>
        /// Cria as tabelas no banco de dados.
        /// </summary>
        /// <returns>awaitable</returns>
        protected abstract Task InitializeTablesAsync();

        /// <summary>
        /// Cria os indices no banco de dados.
        /// </summary>
        /// <returns>awaitable</returns>
        protected abstract Task InitializeIndexAsync();

        /// <summary>
        /// Retorna nome da tabela
        /// </summary>
        /// <returns>tabela</returns>
        protected abstract string GetTableName();

        /// <summary>
        /// Cria um banco de dados com o nome especificado.
        /// Caso já exista um banco de dados com esse nome,
        /// nenhuma alteração é realizada.
        /// </summary>
        /// <param name="dbName">Nome do banco de dados</param>
        /// <returns>awaitable</returns>
        protected async Task CreateDbAsync(string dbName)
        {
            var conn = await _factory.ConnectAsync();

            // query para verificar se existe o banco dbName
            var exists = await R.DbList()
                .Contains(db => db == dbName)
                .RunAsync(conn);

            if (!exists)
            {
                // Cria o banco
                await R.DbCreate(dbName)
                    .RunAsync(conn);

                // espera o banco estar disponível
                await R.Db(dbName)
                    .Wait_()
                    .RunAsync(conn);
            }
        }

        /// <summary>
        /// Cria uma tabela em um banco específico.
        /// Caso já exista uma tabela com o mesmo nome,
        /// nenhuma alteração é realizada.
        /// </summary>
        /// <param name="dbName">Nome do banco</param>
        /// <param name="tableName">Nome da tabela</param>
        /// <returns>awaitable</returns>
        protected async Task CreateTableAsync(string dbName, string tableName)
        {
            var conn = await _factory.ConnectAsync();

            // query pra verificar se existe a tabela
            var exists = await R.Db(dbName).TableList()
                .Contains(t => t == tableName)
                .RunAsync(conn);

            if (!exists)
            {
                // Cria tabela
                await R.Db(dbName).TableCreate(tableName)
                    .RunAsync(conn);

                // Wait_() aguarda que a tabela esteja disponível
                await R.Db(dbName).Table(tableName)
                    .Wait_()
                    .RunAsync(conn);
            }
        }

        /// <summary>
        /// Cria um índice em uma tabela, caso este indice
        /// não exista.
        /// </summary>
        /// <param name="dbName">Nome do banco</param>
        /// <param name="tableName">Nome da tabela</param>
        /// <param name="indexName">Nome do índice</param>
        /// <returns>awaitable</returns>
        protected async Task CreateIndexAsync(string dbName, string tableName, string indexName)
        {
            var conn = await _factory.ConnectAsync();

            // query para verificar se existe o index informado
            var exists = await R.Db(dbName).Table(tableName)
                .IndexList()
                .Contains(t => t == indexName)
                .RunAsync(conn);

            if (!exists)
            {
                // cria o index
                await R.Db(dbName).Table(tableName)
                    .IndexCreate(indexName)
                    .RunAsync(conn);

                // agaurda que o index esteja disponivel
                await R.Db(dbName).Table(tableName)
                    .IndexWait(indexName)
                    .RunAsync(conn);
            }
        }

        /// <summary>
        /// Reconfigura a quantidade de shards e replicas para todas
        /// as tabelas do banco
        /// </summary>
        /// <param name="shards">quantidade de shards para cada tabela</param>
        /// <param name="replicas">quantidade de replicas de cada shard</param>
        /// <returns>awaitable</returns>
        public async Task ReconfigureAsync(int shards, int replicas)
        {
            var conn = await _factory.ConnectAsync();

            // obtem as tabelas do banco
            var tables = await R.Db(_dbName)
                .TableList()
                .RunAsync(conn);

            // iteracao nas tabelas do banco
            foreach (string table in tables)
            {
                // configura a quantidade de shards e replicas
                await R.Db(_dbName).Table(table)
                    .Reconfigure()
                    .OptArg("shards", shards)
                    .OptArg("replicas", replicas)
                    .RunAsync(conn);

                // aguarda ate que a table esteja disponivel novamente
                await R.Db(_dbName).Table(table)
                    .Wait_()
                    .RunAsync(conn);
            }
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
                entity.Id = Guid.NewGuid().ToString();

            Cursor<TEntity> all = await R.Db(_dbName).Table(GetTableName())
                .GetAll(entity.Id)[new { index = "id" }]
                .RunAsync<TEntity>(await Instance());

            var entities = all.ToList();

            all.Close();
            all.Dispose();

            if (entities.Count > 0)
            {
                // atualiza entidade
                _ = await R.Db(_dbName).Table(GetTableName())
                    .Get(entities.First().Id)
                    .Update(entity)
                    .RunWriteAsync(await Instance());

                return entity;
            }
            else
            {
                // insere novo registro
                _ = await R.Db(_dbName).Table(GetTableName())
                    .Insert(entity)
                    .RunWriteAsync(await Instance());

                return entity;
            }
        }

        public async Task<TEntity> ReadAsync(string id)
        {
            Cursor<TEntity> all = await R.Db(_dbName).Table(GetTableName())
                .GetAll(id)[new { index = "id" }]
                .RunAsync<TEntity>(await Instance());

            var entities = all.ToList();

            all.Close();
            all.Dispose();

            return entities?.FirstOrDefault();
        }
    }
}