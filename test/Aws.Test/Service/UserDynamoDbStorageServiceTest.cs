using Amazon.DynamoDBv2;
using Aws.Services;
using Common.Models;
using FakeItEasy;
using NUnit.Framework;

namespace Aws.Test.Service;

public class UserDynamoDbStorageServiceTest
{
    private UserDynamoDbStorageService _userDynamoDbStorageService;
    
    [SetUp]
    public void SetUp()
    {
        _userDynamoDbStorageService = new UserDynamoDbStorageService(A.Fake<IAmazonDynamoDB>());
    }
    
    [Test]
    public async Task CreateUser_Should_Create_And_Return_User()
    {
        var userToCreate = new User()
        {
            Username = "Test",
            Password = "Password123",
            Email = "test@example.com"
        };
        var createdUser = await _userDynamoDbStorageService.CreateUser(userToCreate);
        Assert.IsNotEmpty(createdUser.Id);
        Assert.AreEqual(userToCreate.Username, createdUser.Username);
        Assert.AreEqual(userToCreate.Email, createdUser.Email);
    }
}