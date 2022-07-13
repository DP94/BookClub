using Common.Models;

namespace Core.Services;

public interface IUserService
{
    Task<InternalUser> CreateUser(InternalUser user);
    Task<InternalUser?> GetUserById(string userId);

    Task<List<InternalUser>> GetAllUsers();
    Task<InternalUser> UpdateUser(InternalUser user);

    Task<InternalUser> GetUserByUsername(string username);

    string GetHashedPassword(string password, byte[] salt);
}