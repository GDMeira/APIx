using APIx.DTOs;
using APIx.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[ApiController]
[Route("[controller]")]
public class KeysController(KeysService keysService) : ControllerBase
{
    private readonly KeysService _keysService = keysService;

    [HttpPost(Name = "/")]
    public async Task<IActionResult> Post([FromBody] PostKeysDTO postKeysDTO,
                                        [FromHeader(Name = "Authorization")] string? authorization)
    {
        PostKeysDTO response = await _keysService.PostKeys(postKeysDTO, authorization);

        return CreatedAtAction(nameof(Post), response);
    }

    [HttpGet(Name = "/:type/:value")]
    public async Task<IActionResult> Get([FromRoute] string? type, [FromRoute] string? value,
                                        [FromHeader(Name = "Authorization")] string? authorization)
    {
        GetKeysDTO response = await _keysService.GetKeys(type, value, authorization);

        return CreatedAtAction(nameof(Get), response);
    }
}