using Common.Models;
using Common.Models;

namespace Aws.Services;

public interface IBookDynamoDbStorageService
{
    Task<List<Book>> GetBooks();
    Task<Book> GetBookById(string id);

    Task<Book> CreateBook(Book book);

    Task DeleteBook(string id);

    Task<Book> UpdateBook(Book book);
}