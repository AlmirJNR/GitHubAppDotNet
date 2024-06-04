using Microsoft.AspNetCore.Mvc;

namespace GitHubApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> Get() => Task.FromResult<IActionResult>(Ok(new { Status = "Ok" }));
}