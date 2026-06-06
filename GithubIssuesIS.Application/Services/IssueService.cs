using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Interfaces;

namespace GithubIssuesIS.Application.Services;

public class IssueService(IRepository repository)
{
    private readonly IRepository _repository = repository;

    public Task<List<Issue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync<Issue>(cancellationToken);
    }

    public async Task<Issue?> GetByNumberAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        var issues = await _repository.FindAsync<Issue>(
            issue => issue.Number == number,
            cancellationToken);

        return issues.FirstOrDefault();
    }

    public async Task<Issue> CreateAsync(
        Issue issue,
        CancellationToken cancellationToken = default)
    {
        var exists = await _repository.AnyAsync<Issue>(
            existingIssue => existingIssue.Number == issue.Number,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Issue with number {issue.Number} already exists.");
        }

        return await _repository.AddAsync(issue, cancellationToken);
    }

    public async Task<Issue?> UpdateAsync(
        int number,
        Issue updatedIssue,
        CancellationToken cancellationToken = default)
    {
        var issue = await GetByNumberAsync(number, cancellationToken);

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

        await _repository.UpdateAsync(issue, cancellationToken);

        return issue;
    }

    public async Task<bool> DeleteAsync(
        int number,
        CancellationToken cancellationToken = default)
    {
        var issue = await GetByNumberAsync(number, cancellationToken);

        if (issue is null)
        {
            return false;
        }

        await _repository.DeleteAsync(issue, cancellationToken);

        return true;
    }
}
