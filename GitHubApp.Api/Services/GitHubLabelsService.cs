using GitHub.Models;
using GitHub.Repos.Item.Item;
using GitHub.Repos.Item.Item.Labels;
using GitHubApp.Api.Pools;

namespace GitHubApp.Api.Services;

public class GitHubLabelsService
{
    private readonly GitHubLabelsPool _gitHubLabelsPool;

    public GitHubLabelsService(GitHubLabelsPool gitHubLabelsPool)
    {
        _gitHubLabelsPool = gitHubLabelsPool;
    }

    public async Task<Label> GetOrCreateLabel(RepoItemRequestBuilder repos, string labelName, string? labelColor)
    {
        if (_gitHubLabelsPool.Pool.TryGetValue(labelName, out var label))
            return label;

        try
        {
            label = await repos.Labels[labelName].GetAsync();
        }
        catch (BasicError e) when (e.ResponseStatusCode == StatusCodes.Status404NotFound)
        {
            label = await repos.Labels.PostAsync(new LabelsPostRequestBody
            {
                Color = labelColor,
                Name = labelName
            });
        }

        if (label is null || string.IsNullOrWhiteSpace(label.Name))
            throw new NullReferenceException();

        _gitHubLabelsPool.Pool[labelName] = label;
        return label;
    }
}