using System.Linq.Expressions;

namespace GithubIssuesIS.Application.Interfaces;

public interface IRepository
{
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;

    Task<List<TEntity>> GetAllAsync<TEntity>(
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<List<TEntity>> FindAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<bool> AnyAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<TEntity?> GetByIdAsync<TEntity>(
        object id,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<TEntity> AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task UpdateAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task DeleteAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
