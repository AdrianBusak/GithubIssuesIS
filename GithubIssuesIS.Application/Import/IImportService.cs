namespace GithubIssuesIS.Application.Import;

public interface IImportService
{
    Task<ImportResult> ImportJsonAsync(
        string jsonContent,
        CancellationToken cancellationToken = default);

    Task<ImportResult> ImportXmlAsync(
        string xmlContent,
        CancellationToken cancellationToken = default);
}
