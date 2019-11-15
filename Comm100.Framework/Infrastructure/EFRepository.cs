using Comm100.Framework.Domain.Repository;
using Comm100.Framework.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

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

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            _dbContext.SaveChanges();
        }
        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChangesAsync();
        }

        public TEntity Get(TId id)
        {
            return _dbContext.Set<TEntity>().Find(id);
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
            throw new NotImplementedException();
        }
    }
}
