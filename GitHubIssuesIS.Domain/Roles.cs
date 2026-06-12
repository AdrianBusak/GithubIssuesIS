namespace GitHubIssuesIS.Domain;

public static class Roles
{
    public const string User = "User";
    public const string Admin = "Admin";
    public const string UserOrAdmin = User + "," + Admin;
}
