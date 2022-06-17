using Amazon.DynamoDBv2.Model;
using Aws.Util;
using NUnit.Framework;

namespace Aws.Test.Util;

public class DynamoDbUtilityTest
{
    [Test]
    public void GetBookFromAttributes_SetsName_WhenPresent()
    {
        const string expectedName = "test";
        var item = new Dictionary<string, AttributeValue>
        {
            {
                "name", new AttributeValue(expectedName)
            }
        };
        var book = DynamoDbUtility.GetBookFromAttributes(item);
        Assert.AreEqual(expectedName, book.Name);
    }
    
    [Test]
    public void GetBookFromAttributes_SetsId_WhenPresent()
    {
        const string expectedId = "test";
        var item = new Dictionary<string, AttributeValue>
        {
            {
                "bookId", new AttributeValue(expectedId)
            }
        };
        var book = DynamoDbUtility.GetBookFromAttributes(item);
        Assert.AreEqual(expectedId, book.Id);
    }

    [Test]
    public void GetBookFromAttributes_DoesNotSetName_WhenNotPresent()
    {
        var book = DynamoDbUtility.GetBookFromAttributes(new Dictionary<string, AttributeValue>());
        Assert.Null(book.Name);
    }
    
    [Test]
    public void GetBookFromAttributes_DoesNotSetId_WhenNotPresent()
    {
        var book = DynamoDbUtility.GetBookFromAttributes(new Dictionary<string, AttributeValue>());
        Assert.Null(book.Id);
    }
}