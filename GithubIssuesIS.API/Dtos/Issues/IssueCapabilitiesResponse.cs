namespace GithubIssuesIS.API.Dtos.Issues;

public sealed record IssueCapabilitiesResponse(
    string Source,
    bool SupportsDelete,
    bool RequiresNumberOnCreate);
