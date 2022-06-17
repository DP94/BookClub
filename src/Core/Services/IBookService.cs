using Core.Models;

namespace Core.Services;

public interface IBookService
{
    List<Book> GetBooks();
    Task<Book> GetBookById(int id);
}