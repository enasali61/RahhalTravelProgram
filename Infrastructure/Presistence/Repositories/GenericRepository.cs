using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.TripAndPlaces;
using Microsoft.EntityFrameworkCore;
using Presistence.Data;

namespace Presistence.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TEntity entity)
        => await _dbContext.Set<TEntity>().AddAsync(entity);

        public async Task<int> CountAsync(Specifications<TEntity> specifications)
        => await SpecificationEvaluator.GetQuery<TEntity>(_dbContext.Set<TEntity>(), specifications).CountAsync();


        public void Delete(TEntity entity)
        => _dbContext.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool AsNoTracking = false)
        {
            if (AsNoTracking && typeof(TEntity) == typeof(Places))
                return await _dbContext.Set<TEntity>().ToListAsync();
            return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications)
            => await SpecificationEvaluator.GetQuery<TEntity>(_dbContext.Set<TEntity>(), specifications).ToListAsync();
        

        public async Task<TEntity?> GetByIdAsync(int id)
       => await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(Specifications<TEntity> specifications)
        => await SpecificationEvaluator.GetQuery<TEntity>(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();

        public void Update(TEntity entity)
        =>_dbContext.Set<TEntity>().Update(entity);
    }
}
