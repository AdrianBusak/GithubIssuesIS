namespace GitHubIssuesIS.Client.Models;

public sealed class ImportResultDto
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = [];
}
