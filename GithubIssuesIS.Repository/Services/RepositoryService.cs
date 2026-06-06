using GithubIssuesIS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace GithubIssuesIS.Repository.Services;

public class RepositoryService(
    GithubIssuesIsDbContext dbContext,
    ILogger<RepositoryService> logger)
    : IRepository
{
    private readonly GithubIssuesIsDbContext _dbContext = dbContext;
    private readonly ILogger<RepositoryService> _logger = logger;

    public GithubIssuesIsDbContext DbContext => _dbContext;

    public IQueryable<TEntity> Query<TEntity>()
        where TEntity : class
    {
        return _dbContext.Set<TEntity>().AsQueryable();
    }

    public async Task<List<TEntity>> GetAllAsync<TEntity>(
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await _dbContext.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync<TEntity>(
        object id,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await _dbContext.Set<TEntity>()
            .FindAsync([id], cancellationToken);
    }

    public async Task<List<TEntity>> FindAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await _dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await _dbContext.Set<TEntity>()
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        try
        {
            await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding entity {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        try
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding entity range {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task UpdateAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        try
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating entity {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task DeleteAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        try
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting entity {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
