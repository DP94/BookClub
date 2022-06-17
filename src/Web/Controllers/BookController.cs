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
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(string id)
    {
        var book = await this._bookService.GetBookById(id);
        return Ok(book);
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody]Book book)
    {
        book.Id = Guid.NewGuid().ToString();
        var createdBook = await this._bookService.CreateBook(book);
        return new CreatedResult(createdBook.Id, createdBook);
    }
    
    [HttpPut("{id:int}")]
    public void Put(string id, [FromBody]Book book)
    {
    }
    
    [HttpDelete("{id}")]
    public void Delete(string id)
    {
    }
}