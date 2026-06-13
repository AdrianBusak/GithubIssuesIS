using GitHubIssuesIS.Domain;
using GithubIssuesIS.Application.Interfaces;
using HotChocolate.Authorization;

namespace GithubIssuesIS.API.GraphQL;

public sealed class IssueMutation
{
    [Authorize(Roles = new[] { Roles.Admin })]
    public async Task<IssueGraphQlDto> CreateIssueAsync(
        CreateIssueInput input,
        [Service] IIssueService issueService,
        CancellationToken cancellationToken)
    {
        var issue = await issueService.CreateAsync(input.ToIssue(), cancellationToken);

        return issue.ToGraphQlDto();
    }

    [Authorize(Roles = new[] { Roles.Admin })]
    public async Task<IssueGraphQlDto?> UpdateIssueAsync(
        int number,
        UpdateIssueInput input,
        [Service] IIssueService issueService,
        CancellationToken cancellationToken)
    {
        var issue = await issueService.UpdateAsync(number, input.ToIssue(), cancellationToken);

        return issue?.ToGraphQlDto();
    }

    [Authorize(Roles = new[] { Roles.Admin })]
    public Task<bool> DeleteIssueAsync(
        int number,
        [Service] IIssueService issueService,
        CancellationToken cancellationToken)
    {
        return issueService.DeleteAsync(number, cancellationToken);
    }
}
