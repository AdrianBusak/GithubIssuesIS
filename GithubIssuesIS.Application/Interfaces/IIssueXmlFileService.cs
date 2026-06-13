namespace GithubIssuesIS.Application.Interfaces;

public interface IIssueXmlFileService
{
    Task<string> GenerateXmlFileAsync(
        CancellationToken cancellationToken = default);
}
