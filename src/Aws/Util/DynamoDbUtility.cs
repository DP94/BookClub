using Amazon.DynamoDBv2.Model;
using Common.Models;
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
    
    public static User GetUserFromAttributes(Dictionary<string, AttributeValue> items)
    {
        var user = new User();
        if (items.TryGetValue(DynamoDbConstants.UserIdColName, out var id))
        {
            user.Id = id.S;
        }
        if (items.TryGetValue(DynamoDbConstants.UsernameColName, out var username))
        {
            user.Username = username.S;
        }
        if (items.TryGetValue(DynamoDbConstants.EmailColName, out var email))
        {
            user.Email = email.S;
        }
        return user;
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
    
    public static Meme GetMemeFromAttributes(Dictionary<string, AttributeValue> items)
    {
        var meme = new Meme();
        if (items.TryGetValue(DynamoDbConstants.MemeIdColName, out var id))
        {
            meme.Id = id.S;
        }

        if (items.TryGetValue(DynamoDbConstants.BookIdColName, out var bookId))
        {
            meme.BookId = bookId.S;
        }
        
        if (items.TryGetValue(DynamoDbConstants.MemeImageNameColName, out var imageName))
        {
            meme.ImageName = imageName.S;
        }

        if (items.TryGetValue(DynamoDbConstants.MemeImageKeyColName, out var key))
        {
            meme.S3URL = $"https://{Environment.GetEnvironmentVariable(Constants.S3_BUCKET_NAME)}.s3.eu-west-2.amazonaws.com/{key.S}";
        }
        return meme;
    }
}