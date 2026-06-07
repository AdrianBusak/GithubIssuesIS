using GitHubIssuesIS.Domain.Entities;

namespace GithubIssuesIS.Application.Interfaces;

public interface IIssueRepository : IRepository<Issue>
{
    Task<Issue?> GetByNumberAsync(
        int number,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        int number,
        CancellationToken cancellationToken = default);
}
