using System.Security.Claims;
using APIx.Models;
using APIx.RequestDTOs;
using APIx.ResponseDTOs;
using APIx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIx.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentsController(PaymentsService paymentsService) : ControllerBase
{
    private readonly PaymentsService _paymentsService = paymentsService;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ReqPostPaymentsDTO postPaymentsDTO)
    {
        var claim = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        int paymentProviderId = int.Parse(claim ?? "0");

        ResPostPaymentsDTO payment = await _paymentsService.PostPayment(postPaymentsDTO, paymentProviderId);

        return CreatedAtAction(nameof(Post), payment);
    }
}