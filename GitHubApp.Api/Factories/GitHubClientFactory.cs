using System.Security.Cryptography;
using GitHubApp.Api.Constants;
using GitHubApp.Api.Interfaces;
using GitHubApp.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Octokit;

namespace GitHubApp.Api.Factories;

public class GitHubClientFactory
{
    private readonly GitHubJsonWebTokenOptions _gitHubJsonWebTokenOptions;
    private readonly ICacheService _cache;

    public GitHubClientFactory(IOptions<GitHubJsonWebTokenOptions> jsonWebTokenOptions, ICacheService cache)
    {
        _gitHubJsonWebTokenOptions = jsonWebTokenOptions.Value;
        _cache = cache;
    }

    private static GitHubClient BuildClient(string jwt) => new(new ProductHeaderValue(MiscConstants.AppName))
    {
        Credentials = new Credentials(jwt, AuthenticationType.Bearer)
    };

    public async Task<GitHubClient> CreateClient(long installationId)
    {
        var gitHubInstallationTokenCacheKey = CacheKeysConstants.GitHubInstallationToken(installationId);
        var gitHubInstallationToken = await _cache.Get<string>(gitHubInstallationTokenCacheKey);
        if (!string.IsNullOrWhiteSpace(gitHubInstallationToken))
            return BuildClient(gitHubInstallationToken);

        var gitHubPemSignedJwt = await _cache.Get<string>(CacheKeysConstants.GitHubPemSignedJwt);
        if (string.IsNullOrWhiteSpace(gitHubPemSignedJwt))
        {
            var pemSignedJwtIssuedAt = DateTime.UtcNow;
            var pemSignedJwtIssuedAtExpiresAt = pemSignedJwtIssuedAt.AddMinutes(10);
            var rsa = RSA.Create();
            var pemFile = await File.ReadAllTextAsync(_gitHubJsonWebTokenOptions.PemFileLocation);
            rsa.ImportFromPem(pemFile);
            gitHubPemSignedJwt = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
            {
                Expires = pemSignedJwtIssuedAtExpiresAt,
                Issuer = _gitHubJsonWebTokenOptions.ClientId,
                IssuedAt = pemSignedJwtIssuedAt,
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            });

            _ = await _cache.Set(CacheKeysConstants.GitHubPemSignedJwt, gitHubPemSignedJwt, TimeSpan.FromMinutes(10));
        }

        var pemSignedGitHubClient = BuildClient(gitHubPemSignedJwt);
        var installation = await pemSignedGitHubClient.GitHubApps.GetInstallationForCurrent(installationId);
        var installationToken = await pemSignedGitHubClient.GitHubApps.CreateInstallationToken(installation.Id);
        _ = await _cache.Set(gitHubInstallationTokenCacheKey, installationToken.Token, TimeSpan.FromHours(1));
        return BuildClient(installationToken.Token);
    }
}