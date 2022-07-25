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

    public async Task<User> CreateUser(InternalUser user)
    {
        user.Id = Guid.NewGuid().ToString();
        CreateHashedAndSaltedPassword(user, user.Password);
        return InternalUserToUser(await _userDynamoDbStorageService.CreateUser(user));
    }
    
    private void CreateHashedAndSaltedPassword(InternalUser createdUser, string password)
    {
        var salt = new byte[128 / 8];
        using var rngCsp = RandomNumberGenerator.Create();
        rngCsp.GetNonZeroBytes(salt);
        createdUser.Password = this.GetHashedPassword(password, salt);
        createdUser.Salt = Convert.ToBase64String(salt);
    }
   
    public async Task<User?> GetUserById(string userId)
    {
        var user = await GetInternalUserById(userId);
        return user == null ? null : InternalUserToUser(user);
    }
    
    private async Task<InternalUser?> GetInternalUserById(string userId)
    {
        return await _userDynamoDbStorageService.GetUserById(userId);
    }

    public async Task<List<User>> GetAllUsers()
    {
        var users = await this._userDynamoDbStorageService.GetAllUsers();
        return users.Select(InternalUserToUser).ToList();
    }

    public async Task<User?> UpdateUser(string userId, InternalUser user)
    {
        var latestUser = await GetInternalUserById(userId);
        if (latestUser == null)
        {
            return null;
        }
        latestUser.Name = user.Name;
        latestUser.Email = user.Email;
        latestUser.Loyalty = user.Loyalty;
        latestUser.Username = user.Username;
        latestUser.ProfilePicImage = user.ProfilePicImage;
        latestUser.BooksRead = user.BooksRead;
        if (!string.IsNullOrEmpty(user.Password))
        {
            latestUser.Password = user.Password;
        }
        return InternalUserToUser(await this._userDynamoDbStorageService.UpdateUser(user));
    }
    
    //Purposefully exclude passwords
    private static User InternalUserToUser(InternalUser user)
    {
        return new User
        {
            Email = user.Email,
            Id = user.Id,
            Username = user.Username,
            BooksRead = user.BooksRead,
            Name = user.Name,
            Loyalty = user.Loyalty,
            ProfilePictureS3Url = user.ProfilePictureS3Url
        };
    }

    public async Task<InternalUser> GetInternalUserByUsername(string username)
    {
        return await this._userDynamoDbStorageService.GetUserByUsername(username);
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