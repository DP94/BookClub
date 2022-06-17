using BookClub.Controllers;
using Core.Models;
using NUnit.Framework;

namespace Web.Test;

public class BookControllerTest
{

    private BookController _bookController;
    
    [SetUp]
    public void Setup()
    {
        _bookController = new BookController();
    }
    
    [Test]
    public void Get_Returns_Correct_Book_Details()
    {
        var book = new Book(1, "TestBook");
    }
}