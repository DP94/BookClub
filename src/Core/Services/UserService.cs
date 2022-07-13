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

    public Task<InternalUser> CreateUser(InternalUser user)
    {
        var createdUser = new InternalUser
        {
            Id = Guid.NewGuid().ToString(),
            Username = user.Username,
            Email = user.Email,
        };
        CreateHashedAndSaltedPassword(createdUser, user.Password);
        return _userDynamoDbStorageService.CreateUser(createdUser);
    }
    
    private void CreateHashedAndSaltedPassword(InternalUser createdUser, string password)
    {
        var salt = new byte[128 / 8];
        using var rngCsp = RandomNumberGenerator.Create();
        rngCsp.GetNonZeroBytes(salt);
        createdUser.Password = this.GetHashedPassword(password, salt);
        createdUser.Salt = Convert.ToBase64String(salt);
    }
    
    public Task<InternalUser?> GetUserById(string userId)
    {
        return _userDynamoDbStorageService.GetUserById(userId);
    }

    public Task<List<InternalUser>> GetAllUsers()
    {
        return this._userDynamoDbStorageService.GetAllUsers();
    }

    public Task<InternalUser> UpdateUser(InternalUser user)
    {
        return this._userDynamoDbStorageService.UpdateUser(user);
    }

    public Task<InternalUser> GetUserByUsername(string username)
    {
        return this._userDynamoDbStorageService.GetUserByUsername(username);
    }

    public string GetHashedPassword(string password, byte[] salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password ?? throw new InvalidOperationException(),
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256/8
        ));
    }
}