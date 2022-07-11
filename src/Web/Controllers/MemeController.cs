using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers;

[EnableCors]
[Route("v1/book/{bookId}/[controller]")]
public class MemeController : ControllerBase
{
    private readonly IMemeService _memeService;
    
    public MemeController(IMemeService memeService, IHttpContextAccessor httpContextAccessor)
    {
        this._memeService = memeService;
    }

    [HttpGet("{memeId}")]
    [SwaggerResponse(200, "Success", typeof(Meme))]
    [SwaggerOperation("Gets all memes")]
    public async Task<IActionResult> Get(string bookId, string memeId)
    {
        var books = await this._memeService.GetMemes(bookId);
        return Ok(books);
    }
    
    [HttpGet]
    [SwaggerResponse(200, "Success", typeof(Meme))]
    [SwaggerOperation("Gets all memes")]
    public async Task<IActionResult> Get(string bookId)
    {
        var books = await this._memeService.GetMemes(bookId);
        return Ok(books);
    }

}