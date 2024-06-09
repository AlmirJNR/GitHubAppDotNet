using System.Collections.Concurrent;
using GitHub;

namespace GitHubApp.Api.Pools;

public class GitHubClientPool
{
    public readonly ConcurrentDictionary<long, GitHubClient> Pool = new();
}