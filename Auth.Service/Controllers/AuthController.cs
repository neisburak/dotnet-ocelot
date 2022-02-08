using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("sign-in")]
    public AuthToken? Post([FromBody] User user)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
                 notBefore: now,
                 expires: now.Add(TimeSpan.FromMinutes(2)),
                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["SecretKey"])), SecurityAlgorithms.HmacSha256)
             );

        return new AuthToken
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
            Expires = TimeSpan.FromMinutes(2).TotalSeconds
        };
    }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthToken
{
    public string AccessToken { get; set; }
    public double Expires { get; set; }
}
