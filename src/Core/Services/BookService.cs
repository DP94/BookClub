using Aws.Services;
using Core.Models;

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
}