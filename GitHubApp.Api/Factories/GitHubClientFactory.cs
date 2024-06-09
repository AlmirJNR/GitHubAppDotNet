using System.Security.Cryptography;
using GitHub;
using GitHub.Octokit.Client;
using GitHub.Octokit.Client.Authentication;
using GitHubApp.Api.Options;
using GitHubApp.Api.Pools;
using Microsoft.Extensions.Options;

namespace GitHubApp.Api.Factories;

public class GitHubClientFactory
{
    private readonly GitHubJsonWebTokenOptions _gitHubJsonWebTokenOptions;
    private readonly GitHubClientPool _gitHubClientPool;

    public GitHubClientFactory(
        IOptions<GitHubJsonWebTokenOptions> jsonWebTokenOptions,
        GitHubClientPool gitHubClientPool
    )
    {
        _gitHubClientPool = gitHubClientPool;
        _gitHubJsonWebTokenOptions = jsonWebTokenOptions.Value;
    }

    public GitHubClient GetOrCreateClient(long installationId)
    {
        if (_gitHubClientPool.Pool.TryGetValue(installationId, out var gitHubClient))
            return gitHubClient;

        var rsa = RSA.Create();
        var pemFile = File.ReadAllText(_gitHubJsonWebTokenOptions.PemFileLocation);
        rsa.ImportFromPem(pemFile);
        var requestAdapter = RequestAdapter.Create(new AppInstallationAuthProvider(
            new AppInstallationTokenProvider(
                _gitHubJsonWebTokenOptions.ClientId,
                rsa,
                installationId.ToString(),
                new GitHubAppTokenProvider()
            )
        ));

        var newGitHubClient = new GitHubClient(requestAdapter);
        _gitHubClientPool.Pool[installationId] = newGitHubClient;
        return newGitHubClient;
    }
}