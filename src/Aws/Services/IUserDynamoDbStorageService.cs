using Common.Models;

namespace Aws.Services;

public interface IUserDynamoDbStorageService
{
    Task<User> CreateUser(User user);
}