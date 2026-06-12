using GitHubIssuesIS.Domain;
using GitHubIssuesIS.Domain.Entities;
using GithubIssuesIS.Application.Helpers;
using GithubIssuesIS.Application.Interfaces;

namespace GithubIssuesIS.Application.Services;

public class AuthSeeder(IUserRepository userRepository) : IAuthSeeder
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureUserAsync("user", "User123!", Roles.User, cancellationToken);
        await EnsureUserAsync("admin", "Admin123!", Roles.Admin, cancellationToken);
    }

    private async Task EnsureUserAsync(
        string username,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(
            username,
            cancellationToken);

        if (existingUser is not null)
        {
            return;
        }

        var passwordHash = PasswordHelper.HashPassword(password);

        await _userRepository.AddAsync(
            new User
            {
                Username = username,
                PasswordHash = passwordHash.Hash,
                PasswordSalt = passwordHash.Salt,
                Role = role,
                CreatedAt = DateTime.UtcNow
            },
            cancellationToken);
    }
}
