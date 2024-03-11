using System.Net;
using System.Security.Claims;
using APIx.Exceptions;
using APIx.RequestDTOs;
using APIx.ResponseDTOs;
using APIx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class KeysController(KeysService keysService) : ControllerBase
{
    private readonly KeysService _keysService = keysService;

    [HttpPost(Name = "/")]
    public async Task<IActionResult> Post([FromBody] ReqPostKeysDTO postKeysDTO)
    {
        var claim = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        int paymentProviderId = int.Parse(claim ?? "0");

        ResPostKeysDTO response = await _keysService.PostKeys(postKeysDTO, paymentProviderId);

        return CreatedAtAction(nameof(Post), response);
    }

    [HttpGet("/{type}/{value}")]
    public async Task<IActionResult> Get([FromRoute] string? type, [FromRoute] string? value,
                                        [FromHeader(Name = "Authorization")] string? authorization)
    {
        if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(value))
        {
            throw new AppException(HttpStatusCode.BadRequest, "Type and value are required");
        }
        
        ResGetKeysDTO response = await _keysService.GetKeys(type, value, authorization);

        return Ok(response);
    }
}