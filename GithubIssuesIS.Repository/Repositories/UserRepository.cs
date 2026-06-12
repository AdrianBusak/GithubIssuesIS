using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GithubIssuesIS.Repository.Repositories;

public class UserRepository(GithubIssuesIsDbContext dbContext)
    : BaseRepository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(
        string refreshTokenHash,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(
                refreshToken => refreshToken.TokenHash == refreshTokenHash,
                cancellationToken);
    }

    public async Task AddRefreshTokenAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await DbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SaveChangesAsync(cancellationToken);
    }
}
