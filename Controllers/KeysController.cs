using APIx.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[ApiController]
[Route("[controller]")]
public class KeysController : ControllerBase
{
    [HttpPost(Name = "/")]
    public IActionResult Post([FromBody] PostKeysDTO keys)
    {
        return Ok("I'm alive!");
    }
}