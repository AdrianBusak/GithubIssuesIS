namespace GitHubIssuesIS.Domain.Entities;

public class Issue
{
    public int Id { get; set; }

    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Body { get; set; }

    public string State { get; set; } = "open";

    public string? AuthorLogin { get; set; }

    public string? HtmlUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ClosedAt { get; set; }
}
