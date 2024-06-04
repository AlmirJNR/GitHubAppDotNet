namespace GitHubApp.Api.Options;

public class GitHubJsonWebTokenOptions
{
    public string ClientSecret { get; }
    public string ClientId { get; }

    public GitHubJsonWebTokenOptions(string clientSecret, string clientId)
    {
        ClientSecret = clientSecret;
        ClientId = clientId;
    }
}