using GitHubIssuesIS.Domain;
using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Auth;
using GithubIssuesIS.Application.Helpers;
using GithubIssuesIS.Application.Interfaces;

namespace GithubIssuesIS.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    ITokenHelper tokenHelper) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenHelper _tokenHelper = tokenHelper;

    public async Task<AuthResult> RegisterAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        username = username.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return AuthResult.Failed("Username and password are required.");
        }

        var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);

        if (existingUser is not null)
        {
            return AuthResult.Failed("Username is already taken.");
        }

        var passwordHash = PasswordHelper.HashPassword(password);
        var user = new User
        {
            Username = username,
            PasswordHash = passwordHash.Hash,
            PasswordSalt = passwordHash.Salt,
            Role = Roles.User,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return await CreateTokenPairAsync(user, cancellationToken);
    }

    public async Task<AuthResult> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        username = username.Trim();

        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user is null ||
            !PasswordHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            return AuthResult.Failed("Invalid username or password.");
        }

        return await CreateTokenPairAsync(user, cancellationToken);
    }

    public async Task<AuthResult> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenHelper.HashRefreshToken(refreshToken);
        var storedRefreshToken = await _userRepository.GetRefreshTokenAsync(
            refreshTokenHash,
            cancellationToken);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return AuthResult.Failed("Refresh token is invalid.");
        }

        var nextRefreshToken = _tokenHelper.GenerateRefreshToken();
        storedRefreshToken.RevokedAt = DateTime.UtcNow;

        await _userRepository.AddRefreshTokenAsync(
            new RefreshToken
            {
                UserId = storedRefreshToken.UserId,
                TokenHash = nextRefreshToken.TokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = nextRefreshToken.ExpiresAt
            },
            cancellationToken);

        var accessToken = _tokenHelper.GenerateAccessToken(storedRefreshToken.User);

        return AuthResult.Success(
            accessToken.Token,
            nextRefreshToken.Token,
            storedRefreshToken.User.Username,
            storedRefreshToken.User.Role,
            accessToken.ExpiresAt,
            nextRefreshToken.ExpiresAt);
    }

    public async Task SignOutAsync(
        string? refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var refreshTokenHash = _tokenHelper.HashRefreshToken(refreshToken);
        var storedRefreshToken = await _userRepository.GetRefreshTokenAsync(
            refreshTokenHash,
            cancellationToken);

        if (storedRefreshToken is null || storedRefreshToken.RevokedAt is not null)
        {
            return;
        }

        storedRefreshToken.RevokedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResult> CreateTokenPairAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var accessToken = _tokenHelper.GenerateAccessToken(user);
        var refreshToken = _tokenHelper.GenerateRefreshToken();

        await _userRepository.AddRefreshTokenAsync(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshToken.TokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = refreshToken.ExpiresAt
            },
            cancellationToken);

        return AuthResult.Success(
            accessToken.Token,
            refreshToken.Token,
            user.Username,
            user.Role,
            accessToken.ExpiresAt,
            refreshToken.ExpiresAt);
    }
}
