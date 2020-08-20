using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECS.Framework.Data
{
    public interface IRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Entidade();
        TEntity ObterPeloId(object id);
        Task<TEntity> ObterPeloIdAsync(object id);
        IEnumerable<TEntity> ListarTodos();
        Task<IEnumerable<TEntity>> ListarTodosAsync();
        IEnumerable<TEntity> ListarTodos(params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> ListarTodosAsync(params Expression<Func<TEntity, object>>[] includes);
        void Salvar(TEntity entidade);
        Task SalvarAsync(TEntity entidade);
        void SalvarTodos(IEnumerable<TEntity> entidades);
        Task SalvarTodosAsync(IEnumerable<TEntity> entidades);
        void Deletar(object id);
        Task DeletarAsync(object id);
        void DeletarTodos(IEnumerable<TEntity> entidades);
        Task DeletarTodosAsync(IEnumerable<TEntity> entidades);
    }
}
