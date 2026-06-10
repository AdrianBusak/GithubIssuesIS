namespace GitHubIssuesIS.Client.Models;

public sealed class IssueDto
{
    public int Id { get; set; }

    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Body { get; set; }

    public string State { get; set; } = "open";

    public string? AuthorLogin { get; set; }

    public string? HtmlUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }
}
