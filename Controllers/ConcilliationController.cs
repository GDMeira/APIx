using System.Security.Claims;
using APIx.RequestDTOs;
using APIx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ConcilliationController(ConcilliationService concilliationService) : ControllerBase
{
    private readonly ConcilliationService _concilliationService = concilliationService;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReqPostConcilliationDTO req)
    {
        var claim = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        int paymentProviderId = int.Parse(claim ?? "0");
        var response = await _concilliationService.PostConcilliation(req, paymentProviderId);

        return CreatedAtAction(nameof(Post), response);
    }
}