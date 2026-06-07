using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GithubIssuesIS.Repository.Repositories;

public class IssueRepository(GithubIssuesIsDbContext dbContext)
    : BaseRepository<Issue>(dbContext), IIssueRepository
{
    public async Task<Issue?> GetByNumberAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(issue => issue.Number == number, cancellationToken);
    }

    public async Task<bool> ExistsByNumberAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(issue => issue.Number == number, cancellationToken);
    }
}
