using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using GitHubIssuesIS.Client.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace GitHubIssuesIS.Client.Services;

public class SoapClient(HttpClient httpClient)
{
    private const string SoapAction =
        "http://tempuri.org/IIssueSoapService/SearchIssues";

    private readonly HttpClient _httpClient = httpClient;

    public async Task<SearchIssuesResponseDto> SearchIssuesAsync(string searchTerm)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "IssueSoapService.svc")
        {
            Content = new StringContent(
                CreateEnvelope(searchTerm),
                Encoding.UTF8,
                "text/xml")
        };

        request.Headers.Add("SOAPAction", $"\"{SoapAction}\"");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        using var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(content);
        }

        return ParseResponse(content);
    }

    private static string CreateEnvelope(string searchTerm)
    {
        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
        XNamespace tempuri = "http://tempuri.org/";

        var envelope = new XDocument(
            new XElement(
                soap + "Envelope",
                new XElement(
                    soap + "Body",
                    new XElement(
                        tempuri + "SearchIssues",
                        new XElement(tempuri + "searchTerm", searchTerm)))));

        return envelope.ToString(SaveOptions.DisableFormatting);
    }

    private static SearchIssuesResponseDto ParseResponse(string content)
    {
        var document = XDocument.Parse(content);
        var issueElements = document
            .Descendants()
            .Where(element => element.Name.LocalName == "SoapIssueDto")
            .ToList();

        var issues = issueElements
            .Select(ParseIssue)
            .ToList();

        var count = ParseInt(document
            .Descendants()
            .FirstOrDefault(element => element.Name.LocalName == "Count")
            ?.Value);

        return new SearchIssuesResponseDto
        {
            Count = count == 0 ? issues.Count : count,
            Issues = issues
        };
    }

    private static SoapIssueDto ParseIssue(XElement element)
    {
        return new SoapIssueDto
        {
            Number = ParseInt(GetValue(element, "Number")),
            Title = GetValue(element, "Title") ?? string.Empty,
            Body = NullIfEmpty(GetValue(element, "Body")),
            State = GetValue(element, "State") ?? string.Empty,
            AuthorLogin = NullIfEmpty(GetValue(element, "AuthorLogin")),
            HtmlUrl = NullIfEmpty(GetValue(element, "HtmlUrl")),
            CreatedAt = ParseDateTime(GetValue(element, "CreatedAt")),
            ClosedAt = ParseNullableDateTime(GetValue(element, "ClosedAt"))
        };
    }

    private static string? GetValue(
        XElement element,
        string name)
    {
        return element
            .Elements()
            .FirstOrDefault(child => child.Name.LocalName == name)
            ?.Value;
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
}
