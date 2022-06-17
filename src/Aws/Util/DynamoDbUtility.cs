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
        return book;
    }
}