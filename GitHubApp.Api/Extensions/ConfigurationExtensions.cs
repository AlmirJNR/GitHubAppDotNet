namespace GitHubApp.Api.Extensions;

public static class ConfigurationExtensions
{
    public static T GetRequired<T>(this ConfigurationManager configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new NullReferenceException($"Missing value for key \"{value}\"");

        return (T)Convert.ChangeType(value, typeof(T));
    }
}