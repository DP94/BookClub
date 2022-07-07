using System.Net.Http.Headers;
using Castle.Core.Internal;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers;

[Route("v1/[controller]")]
[EnableCors]
public class BookController : ControllerBase
{

    private readonly IBookService _bookService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public BookController(IBookService service, IHttpContextAccessor httpContextAccessor)
    {
        this._bookService = service;
        this._httpContextAccessor = httpContextAccessor;
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
        return new CreatedResult($"{this._httpContextAccessor.HttpContext?.Request.GetEncodedUrl()}/{createdBook.Id}",
            createdBook);
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

    [HttpPost("{id}/meme")]
    [SwaggerOperation("Creates a new book meme")]
    [SwaggerResponse(201, "Book meme created successfully", typeof(Book))]
    public async Task<IActionResult> CreateBookMeme(string id)
    {
        var form = await Request.ReadFormAsync();
        if (form.Files.Count == 0)
        {
            return BadRequest();
        }

        var file = form.Files.First();
        if (file.FileName == null)
        {
            return BadRequest();
        }
        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        await using (var stream = new FileStream(fileName, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new CreatedResult("", null);
    }
}