using Aws.Services;
using Common.Models;
using Core.Services;
using FakeItEasy;
using NUnit.Framework;

namespace Core.Test;

public class UserServiceTest
{
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _userService = new UserService(A.Fake<IUserDynamoDbStorageService>());
    }

    [Test]
    public async Task Create_Should_Create_And_Return_User_With_Hashed_Password_And_Salt()
    {
        var userToCreate = new User() { Password = "Test123"};
        var createdUser = await _userService.CreateUser(userToCreate);
        Assert.IsNotNull(createdUser);
        Assert.IsNotEmpty(createdUser.Salt);
        Assert.IsNotEmpty(createdUser.Password);
        Assert.AreNotEqual(userToCreate.Password, createdUser.Password);
    }
}