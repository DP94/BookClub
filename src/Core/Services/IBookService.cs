using Core.Models;

namespace Core.Services;

public interface IBookService
{
    Task<List<Book>> GetBooks();
    Task<Book> GetBookById(string id);

    Task<Book> CreateBook(Book book);
}