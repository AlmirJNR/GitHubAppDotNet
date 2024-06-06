namespace GitHubApp.Api.Interfaces;

public interface ICacheService
{
    public Task<T?> Get<T>(string key);
    public Task<bool> Set<T>(string key, T value, TimeSpan ttl);
}