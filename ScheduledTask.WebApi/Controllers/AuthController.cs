using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScheduledTask.WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var validKey = jwtSection["ApiKey"] ?? "admin";
        var secret = jwtSection["Secret"] ?? "ScheduledTaskSecretKey2024!@#$%";

        if (request.ApiKey != validKey)
            return Unauthorized("Invalid API Key");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "ScheduledTask",
            audience: "ScheduledTask",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}

public class LoginRequest
{
    public string ApiKey { get; set; } = string.Empty;
}
