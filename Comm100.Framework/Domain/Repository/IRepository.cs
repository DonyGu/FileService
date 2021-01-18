using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Comm100.Framework.Domain.Specifications;

namespace Comm100.Framework.Domain.Repository
{
    public interface IRepository<TId, TEntity>
    {
        Task<TEntity> Get(TId id);
        IReadOnlyList<TEntity> ListAll();
        IReadOnlyList<TEntity> List(ISpecification<TEntity> spec); 
        Task<int> Count(ISpecification<TEntity> spec);
        Task<TEntity> Create(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TEntity entity);

        Task<bool> Exists(TId id);
    }
}
