using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECS.Framework.Data
{
    public abstract class Repository<TEntity> where TEntity : class
    {
        private DbContext _dbContext;

        protected ILogger<Repository<TEntity>> _logger;
        protected DapperRepositoryUtils _dapperUtils;
        protected CacheWrapper _cache;

        public Repository(DbContext dbContext,
                                ILogger<Repository<TEntity>> logger,
                                DapperRepositoryUtils dapperRepositoryUtils,
                                CacheWrapper cache)
        {
            _dbContext = dbContext;
            _logger = logger;
            _dapperUtils = dapperRepositoryUtils;
            _cache = cache;
        }

        public DbSet<TEntity> Entidade()
        {
            return _dbContext.Set<TEntity>();
        }

        public void Deletar(object id)
        {
            _dbContext.Remove(ObterPeloId(id));
            _dbContext.SaveChanges();
        }
        public async Task DeletarAsync(object id)
        {
            var entidade = await ObterPeloIdAsync(id);
            _dbContext.Remove(entidade);
            await _dbContext.SaveChangesAsync();
        }
        public void DeletarTodos(IEnumerable<TEntity> entidades)
        {
            _dbContext.RemoveRange(entidades);
            _dbContext.SaveChanges();
        }
        public async Task DeletarTodosAsync(IEnumerable<TEntity> entidades)
        {
            _dbContext.RemoveRange(entidades);
            await _dbContext.SaveChangesAsync();
        }

        public IEnumerable<TEntity> ListarTodos()
        {
            return _dbContext.Set<TEntity>();
        }
        public async Task<IEnumerable<TEntity>> ListarTodosAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }
        public IEnumerable<TEntity> ListarTodos(params Expression<Func<TEntity, object>>[] includes)
        {
            var dbSet = _dbContext.Set<TEntity>();
            if (includes != null)
            {
                var query = dbSet.Include(includes[0]);
                for (int i = 1; i < includes.Length; i++)
                {
                    query = query.Include(includes[i]);
                }
                return query;
            }
            return dbSet;
        }

        public async Task<IEnumerable<TEntity>> ListarTodosAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var dbSet = await _dbContext.Set<TEntity>().ToListAsync();
            foreach (var include in includes)
            {
                await _dbContext.Entry(dbSet).Reference(include.Name).LoadAsync();
            }
            return dbSet;
        }

        public TEntity ObterPeloId(object id)
        {
            return _dbContext.Find<TEntity>(id);
        }
        public async Task<TEntity> ObterPeloIdAsync(object id)
        {
            return await _dbContext.FindAsync<TEntity>(id);
        }

        public void Salvar(TEntity entidade)
        {
            _dbContext.AddOrUpdate(entidade);
            _dbContext.SaveChanges();
        }
        public async Task SalvarAsync(TEntity entidade)
        {
            await _dbContext.AddOrUpdateAsync(entidade);
            await _dbContext.SaveChangesAsync();

        }

        public void SalvarTodos(IEnumerable<TEntity> entidades)
        {
            _dbContext.AddRange(entidades);
            _dbContext.SaveChanges();
        }
        public async Task SalvarTodosAsync(IEnumerable<TEntity> entidades)
        {
            await _dbContext.AddRangeAsync(entidades);
            await _dbContext.SaveChangesAsync();
        }
    }

    public static class ContextExtensions
    {
        public static void AddOrUpdate(this DbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    ctx.Add(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public async static Task AddOrUpdateAsync(this DbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    await ctx.AddAsync(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                case EntityState.Added:
                    await ctx.AddAsync(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
