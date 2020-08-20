using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECS.Framework.Data
{
    public class Service<TEntity> : ServiceBase where TEntity : class
    {
        private IRepository<TEntity> _repository;
        protected ILogger<Service<TEntity>> _logger;

        public Service(IRepository<TEntity> repository, ILogger<Service<TEntity>> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public DbSet<TEntity> Entidade()
        {
            return _repository.Entidade();
        }
        public void Deletar(object id)
        {
            _repository.Deletar(id);
        }
        public async Task DeletarAsync(object id)
        {
            await _repository.DeletarAsync(id);
        }
        public void DeletarTodos(IEnumerable<TEntity> entidades)
        {
            _repository.DeletarTodos(entidades);
        }
        public async Task DeletarTodosAsync(IEnumerable<TEntity> entidades)
        {
            await _repository.DeletarTodosAsync(entidades);
        }
        public IEnumerable<TEntity> ListarTodos()
        {
            return _repository.ListarTodos();
        }
        public async Task<IEnumerable<TEntity>> ListarTodosAsync()
        {
            return await _repository.ListarTodosAsync();
        }
        public IEnumerable<TEntity> ListarTodos(params Expression<Func<TEntity, object>>[] includes)
        {
            return _repository.ListarTodos(includes);
        }
        public async Task<IEnumerable<TEntity>> ListarTodosAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            return await _repository.ListarTodosAsync(includes);
        }
        public TEntity ObterPeloId(object id)
        {
            return _repository.ObterPeloId(id);
        }
        public async Task<TEntity> ObterPeloIdAsync(object id)
        {
            return await _repository.ObterPeloIdAsync(id);
        }
        public void Salvar(TEntity entidade)
        {
            _repository.Salvar(entidade);
        }
        public async Task SalvarAsync(TEntity entidade)
        {
            await _repository.SalvarAsync(entidade);
        }
        public void SalvarTodos(IEnumerable<TEntity> entidades)
        {
            _repository.SalvarTodos(entidades);
        }
        public async Task SalvarTodosAsync(IEnumerable<TEntity> entidades)
        {
            await _repository.SalvarTodosAsync(entidades);
        }
    }
}
