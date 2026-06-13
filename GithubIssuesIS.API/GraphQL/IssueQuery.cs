using GitHubIssuesIS.Domain;
using GithubIssuesIS.Application.Interfaces;
using HotChocolate.Authorization;

namespace GithubIssuesIS.API.GraphQL;

public sealed class IssueQuery
{
    [Authorize(Roles = new[] { Roles.User, Roles.Admin })]
    public async Task<IReadOnlyList<IssueGraphQlDto>> GetIssuesAsync(
        [Service] IIssueService issueService,
        CancellationToken cancellationToken)
    {
        var issues = await issueService.GetAllAsync(cancellationToken);

        return issues
            .OrderBy(issue => issue.Number)
            .Select(issue => issue.ToGraphQlDto())
            .ToList();
    }

    [Authorize(Roles = new[] { Roles.User, Roles.Admin })]
    public async Task<IssueGraphQlDto?> GetIssueByNumberAsync(
        int number,
        [Service] IIssueService issueService,
        CancellationToken cancellationToken)
    {
        var issue = await issueService.GetByNumberAsync(number, cancellationToken);

        return issue?.ToGraphQlDto();
    }
}
