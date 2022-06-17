using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookClub.Controllers;

[Route("v1/[controller]")]
public class BookController : ControllerBase
{

    private IBookService _bookService;
    
    public BookController(IBookService service)
    {
        this._bookService = service;
    }
    
    [HttpGet]
    public IEnumerable<string> Get()
    {
        var book = new Book(1, "");
        return new string[] { "Horus Rising", "False Gods", "Galaxy in Flames", "The Flight of the Eisenstein", "Fulgrim" };
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var book = await this._bookService.GetBookById(id);
        return Ok(book);
    }
    
    [HttpPost]
    public void Post([FromBody]Book book)
    {
    }
    
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]Book book)
    {
    }
    
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}