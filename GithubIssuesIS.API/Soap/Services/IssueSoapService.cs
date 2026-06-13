using System.Globalization;
using System.Xml;
using GithubIssuesIS.API.Soap.Interfaces;
using GithubIssuesIS.API.Soap.Models;
using GithubIssuesIS.Application.Interfaces;

namespace GithubIssuesIS.API.Soap.Services;

public sealed class IssueSoapService(
    IIssueXmlFileService xmlFileService) : IIssueSoapService
{
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";

    private readonly IIssueXmlFileService _xmlFileService = xmlFileService;

    public async Task<SearchIssuesResponse> SearchIssuesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new ArgumentException("Search term is required.");
        }

        var filePath = await _xmlFileService.GenerateXmlFileAsync();
        var document = new XmlDocument
        {
            XmlResolver = null
        };

        document.Load(filePath);

        var xpathTerm = EscapeXPathLiteral(searchTerm.Trim().ToLowerInvariant());
        var xpath =
            "/Issues/Issue[" +
            SearchExpression("Number", xpathTerm) + " or " +
            SearchExpression("Title", xpathTerm) + " or " +
            SearchExpression("Body", xpathTerm) + " or " +
            SearchExpression("State", xpathTerm) + " or " +
            SearchExpression("AuthorLogin", xpathTerm) + " or " +
            SearchExpression("HtmlUrl", xpathTerm) + " or " +
            SearchExpression("CreatedAt", xpathTerm) + " or " +
            SearchExpression("ClosedAt", xpathTerm) +
            "]";

        var nodes = document.SelectNodes(xpath);
        var issues = new List<SoapIssueDto>();

        if (nodes is not null)
        {
            foreach (XmlNode node in nodes)
            {
                issues.Add(MapNode(node));
            }
        }

        return new SearchIssuesResponse
        {
            Count = issues.Count,
            Issues = issues
        };
    }

    private static string SearchExpression(
        string elementName,
        string xpathTerm)
    {
        return $"contains(translate({elementName}, '{Uppercase}', '{Lowercase}'), {xpathTerm})";
    }

    private static SoapIssueDto MapNode(XmlNode node)
    {
        return new SoapIssueDto
        {
            Number = ParseInt(node["Number"]?.InnerText),
            Title = node["Title"]?.InnerText ?? string.Empty,
            Body = NullIfEmpty(node["Body"]?.InnerText),
            State = node["State"]?.InnerText ?? string.Empty,
            AuthorLogin = NullIfEmpty(node["AuthorLogin"]?.InnerText),
            HtmlUrl = NullIfEmpty(node["HtmlUrl"]?.InnerText),
            CreatedAt = ParseDateTime(node["CreatedAt"]?.InnerText),
            ClosedAt = ParseNullableDateTime(node["ClosedAt"]?.InnerText)
        };
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var result)
            ? result
            : 0;
    }

    private static DateTime ParseDateTime(string? value)
    {
        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var result)
            ? result
            : default;
    }

    private static DateTime? ParseNullableDateTime(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : ParseDateTime(value);
    }

    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value;
    }

    private static string EscapeXPathLiteral(string value)
    {
        if (!value.Contains('\''))
        {
            return $"'{value}'";
        }

        if (!value.Contains('"'))
        {
            return $"\"{value}\"";
        }

        var parts = value.Split('\'');

        return "concat(" +
            string.Join(", \"'\", ", parts.Select(part => $"'{part}'")) +
            ")";
    }
}
