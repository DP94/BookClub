using Common.Models;
using Common.Models;
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
        _bookController = new BookController(A.Fake<IBookService>(), A.Fake<IMemeService>(), null);
    }
    
    [Test]
    public void Get_Returns_Correct_Book_Details()
    {
        var book = new Book("1", "TestBook", null, null, null);
        _bookController.Get();
    }
}