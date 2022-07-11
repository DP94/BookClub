using Aws.Services;
using Common.Models;
using Common.Models;

namespace Core.Services;

public class BookService : IBookService
{

    private readonly IBookDynamoDbStorageService _dynamoDbStorageService;

    public BookService(IBookDynamoDbStorageService service)
    {
        this._dynamoDbStorageService = service;
    }
    
    public async Task<List<Book>> GetBooks()
    {
        return await this._dynamoDbStorageService.GetBooks();
    }

    public async Task<Book> GetBookById(string id)
    {
        return await this._dynamoDbStorageService.GetBookById(id);
    }

    public async Task<Book> CreateBook(Book book)
    {
        return await this._dynamoDbStorageService.CreateBook(book);
    }

    public Task DeleteBook(string id)
    {
        return this._dynamoDbStorageService.DeleteBook(id);
    }

    public Task<Book> UpdateBook(Book book)
    {
        return this._dynamoDbStorageService.UpdateBook(book);
    }
}