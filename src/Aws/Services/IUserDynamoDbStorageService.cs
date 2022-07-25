using Common.Models;

namespace Aws.Services;

public interface IUserDynamoDbStorageService
{
    Task<InternalUser> CreateUser(InternalUser user);
    Task<InternalUser?> GetUserById(string ignored);
    Task<List<InternalUser>> GetAllUsers();
    Task<InternalUser>? UpdateUser(InternalUser user);

    Task<InternalUser> GetUserByUsername(string username);
}