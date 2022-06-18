using Castle.Core.Internal;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("v1/[controller]")]
public class BookController : ControllerBase
{

    private readonly IBookService _bookService;
    
    public BookController(IBookService service)
    {
        this._bookService = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await this._bookService.GetBooks();
        return Ok(books);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var book = await this._bookService.GetBookById(id);
        return Ok(book);
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody]Book book)
    {
        if (book.Name.IsNullOrEmpty())
        {
            return BadRequest();
        }
        
        book.Id = Guid.NewGuid().ToString();
        var createdBook = await this._bookService.CreateBook(book);
        return new CreatedResult(createdBook.Id, createdBook);
    }
    
    [HttpPut("{id}")]
    public void Put(string id, [FromBody]Book book)
    {
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await this._bookService.DeleteBook(id);
        return NoContent();
    }
}