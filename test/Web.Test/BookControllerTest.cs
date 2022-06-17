using Core.Models;
using Core.Services;
using FakeItEasy;
using NUnit.Framework;
using Web.Controllers;

namespace Web.Test;

public class BookControllerTest
{

    private BookController _bookController;
    
    [SetUp]
    public void Setup()
    {
        _bookController = new BookController(A.Fake<IBookService>());
    }
    
    [Test]
    public void Get_Returns_Correct_Book_Details()
    {
        var book = new Book("1", "TestBook");
        _bookController.Get();
    }
}