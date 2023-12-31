using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        // Verify the credential
        if (credential.UserName != "admin" || credential.Password != "password")
        {
            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint.");
            return Unauthorized(ModelState);
        }

        // Creating the security context
        var claims = new List<Claim> {
                    new (ClaimTypes.Name, "admin"),
                    new (ClaimTypes.Email, "admin@mywebsite.com"),
                    new ("Department", "HR"),
                    new ("Admin", "true"),
                    new ("Manager", "true"),
                    new ("EmploymentDate", "2023-05-01")
                };

        var expiresAt = DateTime.UtcNow.AddMinutes(10);

        return Ok(new
        {
            access_token = CreateToken(claims, expiresAt),
            expires_at = expiresAt,
        });
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
    {
        var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("SecretKey") ?? string.Empty);

        // generate the JWT token
        var jwt = new JwtSecurityToken(
            claims: claims,
        notBefore: DateTime.UtcNow,
        expires: expireAt,
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public class Credential
{
    public string UserName { get; set; }
    public string Password { get; set; }
}