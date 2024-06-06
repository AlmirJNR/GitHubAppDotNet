using System.Text.Json.Nodes;
using Octokit.Internal;

namespace GitHubApp.Api.Extensions;

public static class JsonExtensions
{
    private static readonly SimpleJsonSerializer Serializer = new();

    /// <summary>
    /// More info can be found here https://octokitnet.readthedocs.io/en/latest/github-apps/#a-note-on-identifying-installation-ids
    /// </summary>
    public static T? OctokitDeserialize<T>(this JsonObject jsonObject)
        => Serializer.Deserialize<T>(jsonObject.ToJsonString());
}