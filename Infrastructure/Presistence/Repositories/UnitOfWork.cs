using System.Collections.Concurrent;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Presistence.Data;

namespace Presistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConcurrentDictionary<string, object> _repositories;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new();
        }
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var typeName = typeof(TEntity).Name; //key
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(typeName, _ => new GenericRepository<TEntity>(_dbContext));
        }
        public DbSet<TEntity> Set<TEntity>() where TEntity : class
       => _dbContext.Set<TEntity>();


        public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();
    }
}
