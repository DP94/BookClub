using Core.Models;

namespace Aws.Services;

public interface IMemeDynamoDbStorageService
{
    Task<List<Meme>> GetMemes(string bookId);
    Task<Meme> Create(Meme meme);
}