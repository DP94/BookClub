using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Common.Util;
using Core.Models;

namespace Aws.Util;

public class DynamoDbUtility
{
    public static Book GetBookFromAttributes(Dictionary<string, AttributeValue> items)
    {
        var book = new Book();
        if (items.TryGetValue(DynamoDbConstants.NameColName, out var name))
        {
            book.Name = name.S;
        }

        if (items.TryGetValue(DynamoDbConstants.BookIdColName, out var id))
        {
            book.Id = id.S;
        }
        
        if (items.TryGetValue(DynamoDbConstants.ImageSourceColName, out var imgSrc))
        {
            book.ImageSource = imgSrc.S;
        }
        
        if (items.TryGetValue(DynamoDbConstants.AuthorColName, out var author))
        {
            book.Author = author.S;
        }
        
        if (items.TryGetValue(DynamoDbConstants.SummarySourceColName, out var summary))
        {
            book.Summary = summary.S;
        }
        return book;
    }

    public static Dictionary<string, AttributeValue> GetAttributesFromBook(Book book)
    {
        var attributeValues = new Dictionary<string, AttributeValue>();
        attributeValues.TryAdd(DynamoDbConstants.NameColName, new AttributeValue(book.Name));
        attributeValues.TryAdd(DynamoDbConstants.BookIdColName, new AttributeValue(book.Id));
        attributeValues.TryAdd(DynamoDbConstants.ImageSourceColName, new AttributeValue(book.ImageSource));
        attributeValues.TryAdd(DynamoDbConstants.AuthorColName, new AttributeValue(book.Author));
        attributeValues.TryAdd(DynamoDbConstants.SummarySourceColName, new AttributeValue(book.Summary));
        return attributeValues;
    }
}