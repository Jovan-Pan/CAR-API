using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DebugController(IWebHostEnvironment env, IOptions<AppSettings> settings) : ControllerBase
{
    [HttpGet("environment")]
    public IActionResult GetEnvironmentName()
    {
        string result = string.Format("App: {0},Version: {1}, Environment: {2}",
            settings.Value.AppName, 
            settings.Value.BuildVersion,
            env.EnvironmentName);
        return Ok(result);
    }
}
