using Microsoft.AspNetCore.Mvc;

namespace GitHubApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public Task<OkObjectResult> Get() => Task.FromResult(Ok(new { Status = "Ok" }));
}