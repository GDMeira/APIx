using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("I'm alive!");
    }
}