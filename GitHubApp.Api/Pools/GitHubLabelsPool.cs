using System.Collections.Concurrent;
using GitHub.Models;

namespace GitHubApp.Api.Pools;

public class GitHubLabelsPool
{
    public readonly ConcurrentDictionary<string, Label> Pool = new();
}