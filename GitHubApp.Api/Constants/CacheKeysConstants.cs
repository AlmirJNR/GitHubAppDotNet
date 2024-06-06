namespace GitHubApp.Api.Constants;

public readonly struct CacheKeysConstants
{
    public const string GitHubPemSignedJwt = "GitHub:PemSignedJwt";

    private const string KGitHubInstallationToken = "GitHub:InstallationJwt:{0}";

    public static string GitHubInstallationToken(long installationId)
        => string.Format(KGitHubInstallationToken, installationId);
}