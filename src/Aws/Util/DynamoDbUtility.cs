using System.Globalization;
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
    
    public static InternalUser GetUserFromAttributes(Dictionary<string, AttributeValue> items)
    {
        var user = new InternalUser();
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
        if (items.TryGetValue(DynamoDbConstants.RealNameColumn, out var realName))
        {
            user.Name = realName.S;
        }
        if (items.TryGetValue(DynamoDbConstants.LoyalistColumn, out var loyalty))
        {
            user.Loyalty = loyalty.S;
        }

        if (items.TryGetValue(DynamoDbConstants.PasswordColName, out var password))
        {
            user.Password = password.S;
        }
        
        if (items.TryGetValue(DynamoDbConstants.SaltColName, out var salt))
        {
            user.Salt = salt.S;
        }

        if (items.TryGetValue(DynamoDbConstants.UsernameProfilePicS3Key, out var key))
        {
            var BucketName = Environment.GetEnvironmentVariable(Constants.S3_BUCKET_NAME) ?? "";
            user.ProfilePictureUrl = $"https://{BucketName}.s3.eu-west-2.amazonaws.com/{key.S}";
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

    public static Dictionary<string, AttributeValue> GetAttributesFromUser(InternalUser user)
    {
        var attributeValues = new Dictionary<string, AttributeValue>();
        attributeValues.TryAdd(DynamoDbConstants.UsernameColName, new AttributeValue(user.Username));
        attributeValues.TryAdd(DynamoDbConstants.UserIdColName, new AttributeValue(user.Id));
        attributeValues.TryAdd(DynamoDbConstants.EmailColName, new AttributeValue(user.Email));
        attributeValues.TryAdd(DynamoDbConstants.RealNameColumn, new AttributeValue(user.Name));
        attributeValues.TryAdd(DynamoDbConstants.LoyalistColumn, new AttributeValue(user.Loyalty));
        if (user.BooksRead.Any())
        {
            attributeValues.TryAdd(DynamoDbConstants.BooksReadColumn,
                new AttributeValue(user.BooksRead.Select(book => book.Id).ToList()));
        }

        attributeValues.TryAdd(DynamoDbConstants.PasswordColName, new AttributeValue(user.Password));
        attributeValues.TryAdd(DynamoDbConstants.SaltColName, new AttributeValue(user.Salt));
        attributeValues.TryAdd(DynamoDbConstants.UsernameProfilePicS3Key, new AttributeValue(user.ProfilePictureUrl));

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
        
        if (items.TryGetValue(DynamoDbConstants.MemeUploadedByColName, out var user))
        {
            meme.UploadedBy = user.S;
        }
        
        if (items.TryGetValue(DynamoDbConstants.MemeCreatedOnColName, out var date))
        {
            meme.CreatedOn = DateTime.ParseExact(date.S, "MM/d/yyyy H:mm:ss", CultureInfo.InvariantCulture);
        }
        return meme;
    }
}