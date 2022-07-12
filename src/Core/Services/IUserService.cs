using Common.Models;

namespace Core.Services;

public interface IUserService
{
    Task<User> CreateUser(User user);
    Task<User?> GetUserById(string userId);

    Task<List<User>> GetAllUsers();
}