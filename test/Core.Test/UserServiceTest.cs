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
        var userToCreate = new User { Password = "Test123"};
        var createdUser = await _userService.CreateUser(userToCreate);
        Assert.IsNotNull(createdUser);
        Assert.IsNotEmpty(createdUser.Salt);
        Assert.IsNotEmpty(createdUser.Password);
        Assert.AreNotEqual(userToCreate.Password, createdUser.Password);
    }

    [Test]
    public async Task GetUserById_Should_Return_Retrieved_User_When_User_Exists()
    {
        var fakeUserId = Guid.NewGuid().ToString();
        var fakeUser = new User
        {
            Id = fakeUserId,
        };
        var fakeDynamoDbStorageService = A.Fake<IUserDynamoDbStorageService>();
        A.CallTo(() => fakeDynamoDbStorageService.GetUserById(A<string>.Ignored)).Returns(fakeUser);
        _userService = new UserService(fakeDynamoDbStorageService);
        var retrievedUser = await _userService.GetUserById(fakeUserId);
        Assert.AreEqual(fakeUserId, retrievedUser.Id);
    }
    
    [Test]
    public async Task GetUserById_Should_Return_Null_If_User_Not_Found()
    {
        var fakeDynamoDbStorageService = A.Fake<IUserDynamoDbStorageService>();
        A.CallTo(() => fakeDynamoDbStorageService.GetUserById(A<string>.Ignored)).Returns((User?) null);
        _userService = new UserService(fakeDynamoDbStorageService);
        var retrievedUser = await _userService.GetUserById(Guid.NewGuid().ToString());
        Assert.Null(retrievedUser);
    }
}