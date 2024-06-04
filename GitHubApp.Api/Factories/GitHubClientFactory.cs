using System.Text;
using GitHubApp.Api.Constants;
using GitHubApp.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Octokit;
using Octokit.Internal;

namespace GitHubApp.Api.Factories;

public class GitHubClientFactory
{
    private readonly GitHubJsonWebTokenOptions _gitHubJsonWebTokenOptions;
    private readonly IMemoryCache _cache;

    public GitHubClientFactory(IOptions<GitHubJsonWebTokenOptions> jsonWebTokenOptions, IMemoryCache cache)
    {
        _gitHubJsonWebTokenOptions = jsonWebTokenOptions.Value;
        _cache = cache;
    }

    public GitHubClient CreateClient()
    {
        if (!_cache.TryGetValue("gitHubJwt", out string? gitHubJwt) || string.IsNullOrWhiteSpace(gitHubJwt))
        {
            var issuedAt = DateTime.UtcNow;
            var expiresAt = issuedAt.AddHours(6);
            var securityKey = Encoding.UTF8.GetBytes(_gitHubJsonWebTokenOptions.ClientSecret);
            gitHubJwt = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
            {
                Audience = null,
                Expires = expiresAt,
                Issuer = _gitHubJsonWebTokenOptions.ClientId,
                IssuedAt = issuedAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(securityKey),
                    SecurityAlgorithms.RsaSha256
                )
            });

            _cache.Set("gitHubJwt", gitHubJwt, expiresAt);
        }

        var gitHubClient = new GitHubClient(
            new ProductHeaderValue(MiscConstants.AppName),
            new InMemoryCredentialStore(new Credentials(gitHubJwt, AuthenticationType.Bearer))
        );

        return gitHubClient;
    }
}