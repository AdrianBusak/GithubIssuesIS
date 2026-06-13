using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using GitHubIssuesIS.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace GitHubIssuesIS.Client.Services;

public class ImportClient(
    HttpClient httpClient,
    AuthClient authClient,
    AuthStateService authState,
    NavigationManager navigationManager)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthClient _authClient = authClient;
    private readonly AuthStateService _authState = authState;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task<ImportResultDto> ImportAsync(
        string content,
        string contentType)
    {
        using var response = await SendWithAuthRetryAsync(
            () => new HttpRequestMessage(HttpMethod.Post, "api/import")
            {
                Content = new StringContent(content, Encoding.UTF8, contentType)
            });

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        var result = await response.Content.ReadFromJsonAsync<ImportResultDto>();

        return result ?? new ImportResultDto
        {
            Succeeded = false,
            Message = response.IsSuccessStatusCode
                ? "Import completed, but response could not be parsed."
                : "Import failed."
        };
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
}
