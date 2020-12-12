using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Demo.Core.Entities.Base;
using Demo.Core.Repositories.Base;
using Demo.Core.Specifications.Base;
using Demo.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Infrastructure.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly DemoContext _demoDbContext;
        private readonly IConfiguration _configuration;

        public Repository(DemoContext dbContext, IConfiguration configuration)
        {
            _demoDbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_demoDbContext.Set<T>().AsQueryable(), spec);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _demoDbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> condition)
        {
            return await _demoDbContext.Set<T>().Where(condition).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> condition,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _demoDbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

            if (condition != null) query = query.Where(condition);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> condition,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = _demoDbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (condition != null) query = query.Where(condition);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _demoDbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _demoDbContext.Set<T>().AddAsync(entity);
            await _demoDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _demoDbContext.Entry(entity).State = EntityState.Modified;
            await _demoDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _demoDbContext.Set<T>().Remove(entity);
            await _demoDbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("DemoDbConnection"));
            try
            {
                await connection.OpenAsync(cancellationToken);
                return (await connection.QueryAsync<T>(sql, param, transaction)).ToList();
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                await connection.CloseAsync();
            }

            return null;
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null,
            IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("DemoDbConnection"));
            try
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("DemoDbConnection"));
            try
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleAsync<T>(sql, param, transaction);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_configuration.GetConnectionString("DemoDbConnection"));
            try
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.ExecuteAsync(sql, param, transaction);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}
