using Common.Models;
using Common.Models;

namespace Core.Services;

public interface IBookService
{
    Task<List<Book>> GetBooks();
    Task<Book> GetBookById(string id);

    Task<Book> CreateBook(Book book);

    Task DeleteBook(string id);

    Task<Book> UpdateBook(Book book);
}