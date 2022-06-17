using Core.Models;

namespace Aws.Services;

public interface IBookDynamoDbStorageService
{
    List<Book> GetBooks();
    Task<Book> GetBookById(int id);
}