using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Models;
using Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Web.Controllers;

[Route("v1/login")]
[EnableCors]
public class AuthenticationController: ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    
    
    public AuthenticationController(IUserService userService, IConfiguration config)
    {
        _userService = userService;
        _config = config;
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] InternalUser user)
    {
        try
        {
            var dbUser = await this._userService.GetInternalUserByUsername(user.Username);
            if (string.IsNullOrEmpty(user.Username) || dbUser?.Username != user.Username)
            {
                return Unauthorized();
            }

            var hash = this._userService.GetHashedPassword(user.Password, Convert.FromBase64String(dbUser.Salt));
            if (hash != dbUser.Password)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("UserId", dbUser.Id),
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(this._config["Jwt:Issuer"], this._config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(this._config["Jwt:ExpireTime"])),
                signingCredentials: signIn);

            Response.Headers.Authorization = $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";
            return Ok(dbUser);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
}