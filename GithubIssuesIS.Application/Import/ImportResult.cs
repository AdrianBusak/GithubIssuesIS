namespace GithubIssuesIS.Application.Import;

public sealed record ImportResult(
    bool Succeeded,
    string Message,
    IReadOnlyList<string> Errors)
{
    public static ImportResult Success(string message)
    {
        return new ImportResult(true, message, []);
    }

    public static ImportResult Failure(
        string message,
        IReadOnlyList<string> errors)
    {
        return new ImportResult(false, message, errors);
    }
}
