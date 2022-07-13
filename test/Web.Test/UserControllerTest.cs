using Common.Models;
using Core.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Web.Controllers;

namespace Web.Test;

public class UserControllerTest
{
    private UserController _userController;

    [SetUp]
    public void SetUp()
    {
        _userController = new UserController(A.Fake<IUserService>(),A.Fake<IHttpContextAccessor>());
    }

    [Test]
    public async Task Create_Creates_User_And_Returns_Access_Location_and_Created_User()
    {
        var userToCreate = new InternalUser();
        var result = await _userController.Create(userToCreate);
        Assert.IsInstanceOf<CreatedResult>(result);
        var createdResult = result as CreatedResult;
        Assert.IsNotEmpty(createdResult.Location);
        var createdUser = createdResult.Value as User;
        Assert.IsNotNull(createdUser);
    }

    [Test]
    public async Task Get_Returns_Ok_Result_With_User_If_User_Exists()
    {
        var fakeUserId = Guid.NewGuid().ToString();
        var fakeUser = new InternalUser
        {
            Id = fakeUserId,
        };
        var fakeUserService = A.Fake<IUserService>();
        A.CallTo(() => fakeUserService.GetUserById(A<string>.Ignored)).Returns(fakeUser);
        _userController = new UserController(fakeUserService, A.Fake<IHttpContextAccessor>());
        var result = await _userController.Get(fakeUserId);
        Assert.IsInstanceOf<OkObjectResult>(result);
        var getResult = result as OkObjectResult;
        var retrievedUser = getResult.Value as User;
        Assert.AreEqual(fakeUserId, retrievedUser.Id);
        
    }
    
    [Test]
    public async Task Get_Returns_Not_Found_Result_If_User_Does_Not_Exist()
    {
        var fakeUserService = A.Fake<IUserService>();
        A.CallTo(() => fakeUserService.GetUserById(A<string>.Ignored)).Returns((InternalUser?) null);
        _userController = new UserController(fakeUserService,A.Fake<IHttpContextAccessor>());
        var result = await _userController.Get(Guid.NewGuid().ToString());
        Assert.IsInstanceOf<NotFoundResult>(result);
    }
    
    [Test]
    public async Task GetAll_Returns_Ok_Result()
    {
        var users = new List<InternalUser>
        {
            new()
        };
        var fakeUserService = A.Fake<IUserService>();
        A.CallTo(() => fakeUserService.GetAllUsers()).Returns(users);
        _userController = new UserController(fakeUserService,A.Fake<IHttpContextAccessor>());
        var result = await _userController.GetAllUsers();
        Assert.IsInstanceOf<OkObjectResult>(result);
        var getResult = result as OkObjectResult;
        var retrievedUsers = getResult.Value as List<User>;
        Assert.AreEqual(users, retrievedUsers);
        
    }
}