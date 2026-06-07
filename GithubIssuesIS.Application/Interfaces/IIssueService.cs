using GitHubIssuesIS.Domain.Entities;

namespace GithubIssuesIS.Application.Interfaces;

public interface IIssueService
{
    Task<List<Issue>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Issue?> GetByNumberAsync(
        int number,
        CancellationToken cancellationToken = default);

    Task<Issue> CreateAsync(
        Issue issue,
        CancellationToken cancellationToken = default);

    Task<Issue?> UpdateAsync(
        int number,
        Issue updatedIssue,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int number,
        CancellationToken cancellationToken = default);
}
