using System.Net.Http.Headers;
using System.Net.Http.Json;
using GitHubIssuesIS.Client.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace GitHubIssuesIS.Client.Services;

public class AuthClient(HttpClient httpClient, AuthStateService authState)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly AuthStateService _authState = authState;

    public Task<bool> RegisterAsync(LoginRequestDto request)
    {
        return SendAuthRequestAsync("api/auth/register", request);
    }

    public Task<bool> LoginAsync(LoginRequestDto request)
    {
        return SendAuthRequestAsync("api/auth/login", request);
    }

    public async Task<bool> EnsureAuthenticatedAsync()
    {
        if (_authState.IsAuthenticated)
        {
            return true;
        }

        return await RefreshAsync();
    }

    public async Task<bool> RefreshAsync()
    {
        using var request = CreateRequest(HttpMethod.Post, "api/auth/refresh");
        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _authState.Clear();
            return false;
        }

        return await ApplyAuthResponseAsync(response);
    }

    public async Task SignOutAsync()
    {
        using var request = CreateRequest(HttpMethod.Post, "api/auth/signout");

        if (!string.IsNullOrWhiteSpace(_authState.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _authState.AccessToken);
        }

        try
        {
            using var _ = await _httpClient.SendAsync(request);
        }
        finally
        {
            _authState.Clear();
        }
    }

    private async Task<bool> SendAuthRequestAsync(
        string url,
        LoginRequestDto requestDto)
    {
        using var request = CreateRequest(HttpMethod.Post, url);
        request.Content = JsonContent.Create(requestDto);
        using var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _authState.Clear();
            return false;
        }

        return await ApplyAuthResponseAsync(response);
    }

    private async Task<bool> ApplyAuthResponseAsync(HttpResponseMessage response)
    {
        var authResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
        {
            _authState.Clear();
            return false;
        }

        _authState.SetAuthenticated(authResponse);
        return true;
    }

    private static HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return request;
    }
}
