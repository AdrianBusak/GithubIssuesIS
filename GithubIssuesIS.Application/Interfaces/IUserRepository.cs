using GitHubIssuesIS.Domain.Entities;

namespace GithubIssuesIS.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default);

    Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
