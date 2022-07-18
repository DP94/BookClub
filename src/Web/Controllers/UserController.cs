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
    public async Task<IActionResult> Create([FromBody][SwaggerParameter("The user to create")] InternalUser userToCreate)
    {
        var createdUser = await this._userService.CreateUser(userToCreate);
        var createdUrl = $"{this._contextAccessor.HttpContext?.Request.GetEncodedUrl()}/Get/{createdUser.Id}";
        return new CreatedResult(createdUrl, this.InternalUserToUser(userToCreate));
    }

    [HttpGet("{id}")]
    [SwaggerOperation("Gets a User by their ID")]
    [SwaggerResponse(200, "Success")]
    [SwaggerResponse(404, "User not found")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await this._userService.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(this.InternalUserToUser(user));
    }
    
    [HttpGet]
    [SwaggerOperation("Gets a list of all Users")]
    [SwaggerResponse(200, "Success")]
    public async Task<IActionResult> GetAllUsers()
    {
        var user = await this._userService.GetAllUsers();
        return Ok(user.Select(this.InternalUserToUser).ToList());
    }

    [HttpPut("{id}")]
    [SwaggerOperation("Marks a book as read for this user")]
    [SwaggerResponse(200, "Success")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] InternalUser user)
    {
        var latestUser = await this._userService.GetUserById(id);
        if (latestUser == null)
        {
            return new NotFoundResult();
        }

        latestUser.Name = user.Name;
        latestUser.Email = user.Email;
        latestUser.Loyalty = user.Loyalty;
        latestUser.Username = user.Username;
        if (!string.IsNullOrEmpty(user.Password))
        {
            latestUser.Password = user.Password;
        }

        await this._userService.UpdateUser(latestUser);
        return Ok();
    }

    private InternalUser UserToInternalUser(User user)
    {
        return new InternalUser
        {
            Email = user.Email,
            Id = user.Id,
            Username = user.Username,
            BooksRead = user.BooksRead
        };
    }
    
    //Purposefully exclude passwords
    private User InternalUserToUser(InternalUser user)
    {
        return new User
        {
            Email = user.Email,
            Id = user.Id,
            Username = user.Username,
            BooksRead = user.BooksRead
        };
    }
}