namespace GitHubIssuesIS.Client.Models;

public sealed class IssueCapabilitiesDto
{
    public string Source { get; set; } = string.Empty;

    public bool SupportsDelete { get; set; }

    public bool RequiresNumberOnCreate { get; set; }
}
