namespace GitHubApp.Api.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string value, string? equalsTo)
        => value.Equals(equalsTo, StringComparison.InvariantCultureIgnoreCase);
}