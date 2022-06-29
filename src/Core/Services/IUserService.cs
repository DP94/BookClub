using Common.Models;

namespace Core.Services;

public interface IUserService
{
    Task<User> CreateUser(User user);
}