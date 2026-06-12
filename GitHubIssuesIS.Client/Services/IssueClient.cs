using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GitHubIssuesIS.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace GitHubIssuesIS.Client.Services;

public class IssueClient(
    HttpClient httpClient,
    AuthClient authClient,
    AuthStateService authState,
    NavigationManager navigationManager)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthClient _authClient = authClient;
    private readonly AuthStateService _authState = authState;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task<List<IssueDto>> GetAllAsync()
    {
        using var response = await SendWithAuthRetryAsync(
            () => new HttpRequestMessage(HttpMethod.Get, "api/issues"));

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<IssueDto>>() ?? [];
    }

    public async Task<IssueDto?> GetByNumberAsync(int number)
    {
        using var response = await SendWithAuthRetryAsync(
            () => new HttpRequestMessage(HttpMethod.Get, $"api/issues/{number}"));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IssueDto>();
    }

    public async Task<IssueDto?> CreateAsync(CreateIssueDto dto)
    {
        using var response = await SendWithAuthRetryAsync(
            () => CreateJsonRequest(HttpMethod.Post, "api/issues", dto));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IssueDto>();
    }

    public async Task<IssueDto?> UpdateAsync(int number, UpdateIssueDto dto)
    {
        using var response = await SendWithAuthRetryAsync(
            () => CreateJsonRequest(HttpMethod.Put, $"api/issues/{number}", dto));

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IssueDto>();
    }

    public async Task<bool> DeleteAsync(int number)
    {
        using var response = await SendWithAuthRetryAsync(
            () => new HttpRequestMessage(HttpMethod.Delete, $"api/issues/{number}"));

        return response.IsSuccessStatusCode;
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

    private static HttpRequestMessage CreateJsonRequest<T>(
        HttpMethod method,
        string url,
        T dto)
    {
        return new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(dto)
        };
    }
}
