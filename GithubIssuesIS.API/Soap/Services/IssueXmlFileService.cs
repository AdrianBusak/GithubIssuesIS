using System.Xml.Linq;
using GithubIssuesIS.Application.Interfaces;

namespace GithubIssuesIS.API.Soap.Services;

public sealed class IssueXmlFileService(
    IIssueService issueService,
    IHostEnvironment environment) : IIssueXmlFileService
{
    private readonly IIssueService _issueService = issueService;
    private readonly IHostEnvironment _environment = environment;

    public async Task<string> GenerateXmlFileAsync(
        CancellationToken cancellationToken = default)
    {
        var issues = await _issueService.GetAllAsync(cancellationToken);

        var document = new XDocument(
            new XElement(
                "Issues",
                issues
                    .OrderBy(issue => issue.Number)
                    .Select(issue => new XElement(
                        "Issue",
                        new XElement("Number", issue.Number),
                        new XElement("Title", issue.Title),
                        new XElement("Body", issue.Body ?? string.Empty),
                        new XElement("State", issue.State),
                        new XElement("AuthorLogin", issue.AuthorLogin ?? string.Empty),
                        new XElement("HtmlUrl", issue.HtmlUrl ?? string.Empty),
                        new XElement("CreatedAt", issue.CreatedAt.ToString("O")),
                        new XElement("ClosedAt", issue.ClosedAt?.ToString("O") ?? string.Empty)))));

        var directoryPath = System.IO.Path.Combine(_environment.ContentRootPath, "Generated");
        Directory.CreateDirectory(directoryPath);

        var filePath = System.IO.Path.Combine(directoryPath, "issues.xml");
        await File.WriteAllTextAsync(filePath, document.ToString(), cancellationToken);

        return filePath;
    }
}
