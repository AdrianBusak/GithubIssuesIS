using System.Net.Http.Json;
using GitHubIssuesIS.Client.Models;

namespace GitHubIssuesIS.Client.Services;

public class IssueClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<IssueDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<IssueDto>>("api/issues")
            ?? [];
    }

    public async Task<IssueDto?> GetByNumberAsync(int number)
    {
        var response = await _httpClient.GetAsync($"api/issues/{number}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IssueDto>();
    }

    public async Task<IssueDto?> CreateAsync(CreateIssueDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/issues", dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IssueDto>();
    }

    public async Task<IssueDto?> UpdateAsync(int number, UpdateIssueDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/issues/{number}", dto);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<IssueDto>();
    }

    public async Task<bool> DeleteAsync(int number)
    {
        var response = await _httpClient.DeleteAsync($"api/issues/{number}");

        return response.IsSuccessStatusCode;
    }
}
