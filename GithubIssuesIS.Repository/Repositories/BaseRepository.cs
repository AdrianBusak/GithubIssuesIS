using GithubIssuesIS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GithubIssuesIS.Repository.Repositories;

public abstract class BaseRepository<TEntity>(GithubIssuesIsDbContext dbContext)
    : IRepository<TEntity>
    where TEntity : class
{
    protected GithubIssuesIsDbContext DbContext { get; } = dbContext;

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
