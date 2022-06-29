using Amazon.KeyManagementService;
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
        _userService = new UserService(A.Fake<IUserDynamoDbStorageService>(), A.Fake<IAmazonKeyManagementService>());
    }

    [Test]
    public async Task Create_Should_Create_And_Return_User()
    {
        var userToCreate = new User() { };
        var createdUser = await _userService.CreateUser(userToCreate);
        Assert.IsNotNull(createdUser);
    }
}