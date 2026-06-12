using System.ComponentModel.DataAnnotations;

namespace GithubIssuesIS.API.Dtos.Auth;

public sealed class AuthRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
