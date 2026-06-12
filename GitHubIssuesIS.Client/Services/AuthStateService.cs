using GitHubIssuesIS.Client.Models;

namespace GitHubIssuesIS.Client.Services;

public class AuthStateService
{
    public event Action? Changed;

    public string? AccessToken { get; private set; }

    public string? Username { get; private set; }

    public string? Role { get; private set; }

    public DateTimeOffset? ExpiresAt { get; private set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(AccessToken) &&
        ExpiresAt is not null &&
        ExpiresAt.Value > DateTimeOffset.UtcNow.AddSeconds(10);

    public bool IsAdmin =>
        IsAuthenticated &&
        string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);

    public void SetAuthenticated(LoginResponseDto response)
    {
        AccessToken = response.Token;
        Username = response.Username;
        Role = response.Role;
        ExpiresAt = response.ExpiresAt;
        NotifyChanged();
    }

    public void Clear()
    {
        AccessToken = null;
        Username = null;
        Role = null;
        ExpiresAt = null;
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }
}
