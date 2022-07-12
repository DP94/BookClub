using Common.Models;

namespace Aws.Services;

public interface IUserDynamoDbStorageService
{
    Task<User> CreateUser(User user);
    Task<User?> GetUserById(string ignored);
    Task<List<User>> GetAllUsers();
}