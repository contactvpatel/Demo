using Demo.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Demo.Core.Specifications.Base;

namespace Demo.Core.Repositories.Base
{
    public interface IRepository<T> where T : Entity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> condition);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> condition,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            string includeString = null,
            bool disableTracking = true);

        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> condition,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true);

        Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec);
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync(ISpecification<T> spec);

        Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);

        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null,
            CancellationToken cancellationToken = default);
    }
}