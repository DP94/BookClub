using Aws.Services;
using Core.Models;

namespace Core.Services;

public class BookService : IBookService
{

    private IBookDynamoDbStorageService _dynamoDbStorageService;

    public BookService(IBookDynamoDbStorageService service)
    {
        this._dynamoDbStorageService = service;
    }
    
    public List<Book> GetBooks()
    {
        throw new NotImplementedException();
    }

    public async Task<Book> GetBookById(int id)
    {
        return await this._dynamoDbStorageService.GetBookById(id);
    }
}