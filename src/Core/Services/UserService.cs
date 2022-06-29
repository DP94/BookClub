using System.Security.Cryptography;
using Amazon.KeyManagementService;
using Aws.Services;
using Common.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Core.Services;

public class UserService : IUserService
{
    private readonly IUserDynamoDbStorageService _userDynamoDbStorageService;
    private readonly IAmazonKeyManagementService _keyManagementService;
    public UserService(IUserDynamoDbStorageService userDynamoDbStorageService, IAmazonKeyManagementService keyManagementService)
    {
        _userDynamoDbStorageService = userDynamoDbStorageService;
        _keyManagementService = keyManagementService;
    }

    public Task<User> CreateUser(User user)
    {
        var createdUser = new User()
        {
            Id = Guid.NewGuid().ToString(),
            Username = user.Username,
            Email = user.Email,
            Password = CreateHashedAndSaltedPassword(user.Password)
        };
        return _userDynamoDbStorageService.CreateUser(user);
    }
    
    private string CreateHashedAndSaltedPassword(string password)
    {
        var salt = new byte[128 / 8];
        using var rngCsp = RandomNumberGenerator.Create();
        rngCsp.GetNonZeroBytes(salt);
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256/8
        ));
    }

    private string GetSaltFromAmazonKeyManagement()
    {
        throw new NotImplementedException();
    }
}