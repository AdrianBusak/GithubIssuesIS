using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace GitHubIssuesIS.Client.Services;

public class GraphQLClient(
    HttpClient httpClient,
    AuthClient authClient,
    AuthStateService authState,
    NavigationManager navigationManager)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthClient _authClient = authClient;
    private readonly AuthStateService _authState = authState;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task<string> ExecuteAsync(string query)
    {
        using var response = await SendWithAuthRetryAsync(
            () => CreateJsonRequest(new GraphQlRequest(query)));

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(content);
        }

        return FormatJson(content);
    }

    private async Task<HttpResponseMessage> SendWithAuthRetryAsync(
        Func<HttpRequestMessage> requestFactory)
    {
        if (!_authState.IsAuthenticated)
        {
            await _authClient.RefreshAsync();
        }

        var response = await SendAsync(requestFactory());

        if (response.StatusCode == HttpStatusCode.Unauthorized &&
            await _authClient.RefreshAsync())
        {
            response.Dispose();
            response = await SendAsync(requestFactory());
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _authState.Clear();
            _navigationManager.NavigateTo("/login");
        }

        return response;
    }

    private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        if (!string.IsNullOrWhiteSpace(_authState.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _authState.AccessToken);
        }

        return _httpClient.SendAsync(request);
    }

    private static HttpRequestMessage CreateJsonRequest(GraphQlRequest request)
    {
        return new HttpRequestMessage(HttpMethod.Post, "graphql")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
    }

    private static string FormatJson(string content)
    {
        try
        {
            using var document = JsonDocument.Parse(content);
            return JsonSerializer.Serialize(document.RootElement, JsonOptions);
        }
        catch (JsonException)
        {
            return content;
        }
    }

    private sealed record GraphQlRequest(string Query);
}
