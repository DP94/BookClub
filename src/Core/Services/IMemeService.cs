using Core.Models;

namespace Core.Services;

public interface IMemeService
{

    Task<List<Meme>> GetMemes(string bookId);
    Task<Meme> Create(Meme meme);
}