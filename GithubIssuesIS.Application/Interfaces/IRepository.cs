namespace GithubIssuesIS.Application.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default);

    Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);
}
