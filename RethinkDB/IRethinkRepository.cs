using System.Threading.Tasks;

namespace ECS.Framework.Data.RethinkDB
{
    public interface IRethinkRepository<TEntity> where TEntity : RethinkEntityBase
    {
        /// <summary>
        /// Insere um registro no banco de dados.
        /// </summary>
        /// <param name="entity">Dado que será inserido no banco de dados.</param>
        /// <returns>O objeto com o identificador único criado na base de dados.</returns>
        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        /// <summary>
        /// Busca e retornar os dados de um registro a partir de um identificador único (Id).
        /// </summary>
        /// <param name="id">Identificador único do registro no banco de dados.</param>
        /// <returns>Dados do registro encontrado.</returns>
        Task<TEntity> ReadAsync(string id);
    }
}