using Common.Models;

namespace Core.Services;

public interface IUserService
{
    Task<User> CreateUser(InternalUser user);
    Task<User?> GetUserById(string userId);
    Task<List<User>> GetAllUsers();
    Task<User?> UpdateUser(string userId, InternalUser user);

    Task<InternalUser> GetInternalUserByUsername(string username);

    string GetHashedPassword(string password, byte[] salt);
}