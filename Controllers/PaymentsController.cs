using System.Security.Claims;
using APIx.RequestDTOs;
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
    public IActionResult Post([FromBody] ReqPostPaymentsDTO postPaymentsDTO)
    {
        var claim = HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        int paymentProviderId = int.Parse(claim ?? "0");

        _ = _paymentsService.PostPayment(postPaymentsDTO, paymentProviderId);

        return Created();
    }
}