using System.Security.Claims;
using System.Text.Encodings.Web;
using APIx.Models;
using APIx.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace APIx.Helpers;

public class BearerAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AuthRepository authRepository) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly AuthRepository _authRepository = authRepository;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {            
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        string? authorizationHeader = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        if (!authorizationHeader.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        string token = authorizationHeader.Split(" ")[1]; //Bearer token

        if (string.IsNullOrEmpty(token))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        PaymentProvider? paymentProvider = await _authRepository.RetrievePaymentProviderByToken(token);
        
        if (paymentProvider == null)
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, paymentProvider.Id.ToString()),
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}