using Comm100.Framework.Domain.Repository;
using Comm100.Framework.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comm100.Framework.Infrastructure
{
    public class EFRepository<TId, TEntity> : IRepository<TId, TEntity> where TEntity : class
    {
        protected readonly DbContext _dbContext;

        public EFRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int Count(ISpecification<TEntity> spec)
        {
            return ApplySpecification(spec).Count();
        }

        public TEntity Create(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public async Task Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity> Get(TId id)
        {
             return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public bool Exists(TId id)
        {
            return Get(id) != null;
        }

        public IReadOnlyList<TEntity> ListAll()
        {
            return GetQueryable().ToList();
        }

        public IReadOnlyList<TEntity> List(ISpecification<TEntity> spec)
        {
            return ApplySpecification(spec).ToList();
        }

        private IQueryable<TEntity> GetQueryable()
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            return query;
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return SpecificationEvaluator<TEntity>.GetQuery(GetQueryable(), spec);
        }
    }
}
