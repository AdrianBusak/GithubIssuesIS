using GithubIssuesIS.Application.Auth;

namespace GithubIssuesIS.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthResult> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task SignOutAsync(
        string? refreshToken,
        CancellationToken cancellationToken = default);
}
