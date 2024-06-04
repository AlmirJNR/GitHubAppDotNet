using GitHubApp.Api.Constants;
using GitHubApp.Api.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GitHubApp.Api.Extensions;

public static class ServicesExtensions
{
    public static void ConfigureJsonWebTokenOptions(this WebApplicationBuilder builder)
    {
        var securityKey = builder.Configuration.GetRequired<string>(MiscConstants.GitHubClientSecret);
        var clientId = builder.Configuration.GetRequired<string>(MiscConstants.GitHubClientId);
        builder.Services.TryAddSingleton<IOptions<GitHubJsonWebTokenOptions>>(
            new OptionsWrapper<GitHubJsonWebTokenOptions>(
                new GitHubJsonWebTokenOptions(
                    securityKey,
                    clientId
                )
            )
        );
    }
}