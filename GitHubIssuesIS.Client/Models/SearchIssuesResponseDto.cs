namespace GitHubIssuesIS.Client.Models;

public sealed class SearchIssuesResponseDto
{
    public int Count { get; set; }

    public List<SoapIssueDto> Issues { get; set; } = [];
}
