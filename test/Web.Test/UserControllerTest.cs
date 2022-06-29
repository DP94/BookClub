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
        _userController = new UserController(A.Fake<IUserService>(), A.Fake<IHttpContextAccessor>());
    }

    [Test]
    public async Task Create_Creates_User_And_Returns_Access_Location_and_Created_User_Details_Excluding_Password()
    {
        var userToCreate = new User();
        var result = await _userController.Create(userToCreate);
        Assert.IsInstanceOf<CreatedResult>(result);
        var createdResult = result as CreatedResult;
        Assert.IsNotEmpty(createdResult.Location);
        var createdUser = createdResult.Value as User;
    }
}