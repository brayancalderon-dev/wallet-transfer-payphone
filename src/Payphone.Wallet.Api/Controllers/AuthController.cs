using Microsoft.AspNetCore.Mvc;
using Payphone.Wallet.Infrastructure.Auth;

namespace Payphone.Wallet.Api.Controllers;

public sealed record LoginRequest(string Username, string Password);
public sealed record LoginResponse(string Token);

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IConfiguration _configuration;

    public AuthController(
        JwtTokenGenerator tokenGenerator,
        IConfiguration configuration)
    {
        _tokenGenerator = tokenGenerator;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(
        [FromBody] LoginRequest request)
    {
        var validUsername =
            _configuration["AdminCredentials:Username"];

        var validPassword =
            _configuration["AdminCredentials:Password"];

        if (request.Username != validUsername ||
            request.Password != validPassword)
        {
            return Unauthorized(new
            {
                message = "Invalid credentials."
            });
        }

        var token = _tokenGenerator.GenerateToken(request.Username);

        return Ok(new LoginResponse(token));
    }
}