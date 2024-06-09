namespace GitHubApp.Api.Options;

public class GitHubJsonWebTokenOptions
{
    public string ClientId { get; }
    public string PemFileLocation { get; }

    public GitHubJsonWebTokenOptions(string clientId, string pemFileLocation)
    {
        ClientId = clientId;
        PemFileLocation = pemFileLocation
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }
}