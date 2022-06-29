using Common.Models;
using Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers;

[Route("v1/[controller]")]
[EnableCors]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserController(IUserService userService, IHttpContextAccessor contextAccessor)
    {
        _userService = userService;
        _contextAccessor = contextAccessor;
    }

    [HttpPost]
    [SwaggerOperation("Creates a new user")]
    [SwaggerResponse(201, "User created successfully", typeof(User))]
    public async Task<IActionResult> Create(User userToCreate)
    {
        var createdUser = await this._userService.CreateUser(userToCreate);
        var createdUrl = $"{this._contextAccessor.HttpContext?.Request.GetEncodedUrl()}/Get/{createdUser.Id}";
        return new CreatedResult(createdUrl, createdUser);
    }
}