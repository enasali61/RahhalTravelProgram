using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Contracts
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync(); // rows number for affected
        
        // signiture for methods 
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
