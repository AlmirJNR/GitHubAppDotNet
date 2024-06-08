using GitHubApp.Api.Constants;
using GitHubApp.Api.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GitHubApp.Api.Extensions;

public static class ServicesExtensions
{
    public static void ConfigureJsonWebTokenOptions(this WebApplicationBuilder builder)
    {
        var clientId = builder.Configuration.GetRequired<string>(ConfigKeysConstants.GitHubClientId);
        var pemFileLocation = builder.Configuration.GetRequired<string>(ConfigKeysConstants.GitHubPemFileLocation);
        builder.Services.TryAddSingleton<IOptions<GitHubJsonWebTokenOptions>>(
            new OptionsWrapper<GitHubJsonWebTokenOptions>(
                new GitHubJsonWebTokenOptions(clientId, pemFileLocation)
            )
        );
    }
}