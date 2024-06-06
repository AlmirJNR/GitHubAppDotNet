namespace GitHubApp.Api.Constants;

public readonly struct CacheKeysConstants
{
    public const string GitHubPemSignedJwt = "GitHub:PemSignedJwt";

    private const string KGitHubInstallationJwt = "GitHub:InstallationJwt:{0}";

    public static string GitHubInstallationJwt(long installationId)
        => string.Format(KGitHubInstallationJwt, installationId);
}