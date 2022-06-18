using Castle.Core.Internal;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerResponse(200, "Success", typeof(Book))]
    [SwaggerOperation("Gets all books")]
    public async Task<IActionResult> Get()
    {
        var books = await this._bookService.GetBooks();
        return Ok(books);
    }
    
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Success", typeof(Book))]
    [SwaggerOperation("Gets a book by its ID")]
    public async Task<IActionResult> Get(string id)
    {
        var book = await this._bookService.GetBookById(id);
        return Ok(book);
    }
    
    [HttpPost]
    [SwaggerOperation("Creates a new book")]
    [SwaggerResponse(201, "Book created successfully", typeof(Book))]
    public async Task<ActionResult> Post([FromBody][SwaggerParameter("The book to create")] Book book)
    {
        if (book.Name.IsNullOrEmpty())
        {
            return BadRequest();
        }
        
        book.Id = Guid.NewGuid().ToString();
        var createdBook = await this._bookService.CreateBook(book);
        return new CreatedResult(createdBook.Id, createdBook);
    }
    
    [SwaggerOperation("Updates a book")]
    [SwaggerResponse(200, "Book updated", typeof(Book))]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put([SwaggerParameter("ID of the book to update")] string id, [FromBody]Book book)
    {
        var response = await this._bookService.UpdateBook(book);
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    [SwaggerResponse(204, "Book deleted")]
    [SwaggerOperation("Deletes a book")]
    public async Task<IActionResult> Delete([SwaggerParameter("ID of the book to delete")] string id)
    {
        await this._bookService.DeleteBook(id);
        return NoContent();
    }
}