using System.Security.Cryptography;
using Aws.Services;
using Common.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Core.Services;

public class UserService : IUserService
{
    private readonly IUserDynamoDbStorageService _userDynamoDbStorageService;
    public UserService(IUserDynamoDbStorageService userDynamoDbStorageService)
    {
        _userDynamoDbStorageService = userDynamoDbStorageService;
    }

    public Task<User> CreateUser(User user)
    {
        var createdUser = new User()
        {
            Id = Guid.NewGuid().ToString(),
            Username = user.Username,
            Email = user.Email,
        };
        CreateHashedAndSaltedPassword(createdUser, user.Password);
        return _userDynamoDbStorageService.CreateUser(createdUser);
    }
    
    private void CreateHashedAndSaltedPassword(User createdUser, string password)
    {
        var salt = new byte[128 / 8];
        using var rngCsp = RandomNumberGenerator.Create();
        rngCsp.GetNonZeroBytes(salt);
        createdUser.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password ?? throw new InvalidOperationException(),
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256/8
        ));
        createdUser.Salt = Convert.ToBase64String(salt);
    }
    
    public Task<User?> GetUserById(string userId)
    {
        return _userDynamoDbStorageService.GetUserById(userId);
    }
}