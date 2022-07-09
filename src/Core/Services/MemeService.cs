using Aws.Services;
using Core.Models;

namespace Core.Services;

public class MemeService : IMemeService
{
    private readonly IMemeDynamoDbStorageService _memeDynamoDbStorage;
    
    public MemeService(IMemeDynamoDbStorageService memeDynamoDbStorageService)
    {
        this._memeDynamoDbStorage = memeDynamoDbStorageService;
    }

    public async Task<List<Meme>> GetMemes(string bookId)
    {
        return await this._memeDynamoDbStorage.GetMemes(bookId);
    }

    public async Task<Meme> Create(Meme meme)
    {
        return await this._memeDynamoDbStorage.Create(meme);
    }
}