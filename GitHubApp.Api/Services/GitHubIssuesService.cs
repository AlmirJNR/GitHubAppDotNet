namespace GitHubApp.Api.Services;

using GitHub.Repos.Item.Item.Issues.Item;
using GitHub.Repos.Item.Item.Issues.Item.Comments;

public class GitHubIssuesService
{
    public Task AddLabels(
        WithIssue_numberItemRequestBuilder repositoryIssues,
        IEnumerable<string> existingLabels,
        params string[] labels
    )
    {
        return repositoryIssues.PatchAsync(new WithIssue_numberPatchRequestBody
        {
            Labels = existingLabels.Concat(labels).ToList()
        });
    }

    public Task RemoveLabels(
        WithIssue_numberItemRequestBuilder repositoryIssues,
        string[] existingLabels,
        params string[] labelsToRemove
    )
    {
        var remainingLabels = existingLabels.Except(labelsToRemove).ToList();
        if (remainingLabels.Count == existingLabels.Length)
            return Task.CompletedTask;

        return repositoryIssues.PatchAsync(new WithIssue_numberPatchRequestBody
        {
            Labels = remainingLabels
        });
    }

    public Task CreateComment(WithIssue_numberItemRequestBuilder repositoryIssues, string comment)
    {
        return repositoryIssues.Comments.PostAsync(new CommentsPostRequestBody
        {
            Body = comment
        });
    }
}