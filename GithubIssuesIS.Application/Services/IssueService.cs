using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Interfaces;

namespace GithubIssuesIS.Application.Services;

public class IssueService(IIssueRepository issueRepository) : IIssueService
{
    private readonly IIssueRepository _issueRepository = issueRepository;

    public Task<List<Issue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _issueRepository.GetAllAsync(cancellationToken);
    }

    public Task<Issue?> GetByNumberAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        return _issueRepository.GetByNumberAsync(number, cancellationToken);
    }

    public async Task<Issue> CreateAsync(
        Issue issue,
        CancellationToken cancellationToken = default)
    {
        var exists = await _issueRepository.ExistsByNumberAsync(issue.Number, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Issue with number {issue.Number} already exists.");
        }

        return await _issueRepository.AddAsync(issue, cancellationToken);
    }

    public async Task<Issue?> UpdateAsync(
        int number,
        Issue updatedIssue,
        CancellationToken cancellationToken = default)
    {
        var issue = await _issueRepository.GetByNumberAsync(number, cancellationToken);

        if (issue is null)
        {
            return null;
        }

        issue.Title = updatedIssue.Title;
        issue.Body = updatedIssue.Body;
        issue.State = updatedIssue.State;
        issue.AuthorLogin = updatedIssue.AuthorLogin;
        issue.HtmlUrl = updatedIssue.HtmlUrl;
        issue.ClosedAt = updatedIssue.ClosedAt;

        await _issueRepository.UpdateAsync(issue, cancellationToken);

        return issue;
    }

    public async Task<bool> DeleteAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        var issue = await _issueRepository.GetByNumberAsync(number, cancellationToken);

        if (issue is null)
        {
            return false;
        }

        await _issueRepository.DeleteAsync(issue, cancellationToken);

        return true;
    }
}
