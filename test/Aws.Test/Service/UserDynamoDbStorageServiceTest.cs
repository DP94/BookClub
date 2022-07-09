﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Aws.DynamoDbLocal;
using Aws.Services;
using Common.Models;
using FakeItEasy;
using NUnit.Framework;

namespace Aws.Test.Service;

public class UserDynamoDbStorageServiceTest
{
    private UserDynamoDbStorageService _userDynamoDbStorageService;
    private LocalDynamoDbSetup _dynamoDb;
        
    [SetUp]
    public async Task SetUp()
    {
        _dynamoDb = new LocalDynamoDbSetup();
        await _dynamoDb.SetupDynamoDb();
        _userDynamoDbStorageService = new UserDynamoDbStorageService(_dynamoDb.GetClient());
    }
    
    [Test]
    public async Task CreateUser_Should_Create_And_Return_User()
    {
        var userToCreate = GetUser();
        var createdUser = await _userDynamoDbStorageService.CreateUser(userToCreate);
        Assert.IsNotEmpty(createdUser.Id);
        Assert.AreEqual(userToCreate.Username, createdUser.Username);
        Assert.AreEqual(userToCreate.Email, createdUser.Email);
    }

    private User GetUser()
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "Test",
            Password = "Password123",
            Email = "test@example.com",
            Salt = "salty"
        };
    }
    
    [Test]
    public async Task GetUserById_Should_Return_User_If_Existing_User_Matches_Id()
    {
        var userToRetrieve = GetUser();
        await _userDynamoDbStorageService.CreateUser(userToRetrieve);
        var retrievedUser = await _userDynamoDbStorageService.GetUserById(userToRetrieve.Id);
        Assert.AreEqual(userToRetrieve.Id, retrievedUser.Id);
        Assert.AreEqual(userToRetrieve.Username, retrievedUser.Username);
        Assert.AreEqual(userToRetrieve.Email, retrievedUser.Email);
        Assert.AreEqual(userToRetrieve.Password, retrievedUser.Password);
    }
    [Test]
    
    public async Task GetUserById_Should_Return_Null_If_No_Existing_User_Matches_Id()
    {
        var retrievedUser = await _userDynamoDbStorageService.GetUserById(Guid.NewGuid().ToString());
        Assert.Null(retrievedUser);
    }

    [TearDown]
    public void TearDown()
    {
        _dynamoDb.KillProcess();
    }
}