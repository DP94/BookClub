using Common.Models;
using Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("v1/login")]
[EnableCors]
public class AuthenticationController: ControllerBase
{
    private readonly IUserService _userService;
    
    
    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] InternalUser user)
    {
        try
        {
            var dbUser = await this._userService.GetUserByUsername(user.Username);
            if (string.IsNullOrEmpty(user.Username) || dbUser?.Username != user.Username)
            {
                return Unauthorized();
            }

            var hash = this._userService.GetHashedPassword(user.Password, Convert.FromBase64String(dbUser.Salt));
            if (hash != dbUser.Password)
            {
                return Unauthorized();
            }

            return Ok(dbUser);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }
}