using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Aws.Util;
using Common.Models;
using Common.Util;

namespace Aws.Services;

public class UserDynamoDbStorageService : IUserDynamoDbStorageService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IAmazonS3 _s3Client;
    private readonly string BucketName;

    public UserDynamoDbStorageService(IAmazonDynamoDB amazonDynamoDb, IAmazonS3 s3Client)
    {
        this._dynamoDb = amazonDynamoDb;
        this._s3Client = s3Client;
        this.BucketName = Environment.GetEnvironmentVariable(Constants.S3_BUCKET_NAME) ?? "";
    }

    public async Task<InternalUser> CreateUser(InternalUser user)
    {
        await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { DynamoDbConstants.UserIdColName, new AttributeValue(user.Id) },
                { DynamoDbConstants.UsernameColName, new AttributeValue(user.Username) },
                { DynamoDbConstants.EmailColName, new AttributeValue(user.Email) },
                { DynamoDbConstants.RealNameColumn, new AttributeValue(user.Name) },
                { DynamoDbConstants.LoyalistColumn, new AttributeValue(user.Loyalty) },
                { DynamoDbConstants.PasswordColName, new AttributeValue(user.Password) },
                { DynamoDbConstants.SaltColName, new AttributeValue(user.Salt) }
            }
        });
        return user;
    }

    public async Task<InternalUser?> GetUserById(string id)
    {
        var queryResult = await this._dynamoDb.QueryAsync(new QueryRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            KeyConditions = new Dictionary<string, Condition>
            {
                {
                    DynamoDbConstants.UserIdColName,
                    new Condition
                    {
                        ComparisonOperator = ComparisonOperator.EQ,
                        AttributeValueList = { new AttributeValue(id) }
                    }
                }
            }
        });
        var items = queryResult.Items.FirstOrDefault();
        return items == null
            ? null
            : DynamoDbUtility.GetUserFromAttributes(items);
    }

    public async Task<List<InternalUser>> GetAllUsers()
    {
        var result = await this._dynamoDb.ScanAsync(new ScanRequest(DynamoDbConstants.UserTableName));
        var users = new List<InternalUser>();
        foreach (var item in result.Items)
        {
            var user = DynamoDbUtility.GetUserFromAttributes(item);
            users.Add(user);

            if (item.TryGetValue(DynamoDbConstants.BooksReadColumn, out var books))
            {
                var request = new BatchGetItemRequest
                {
                    RequestItems = new Dictionary<string, KeysAndAttributes>
                    {
                        {
                            DynamoDbConstants.BookTableName,
                            new KeysAndAttributes
                            {
                                Keys = new List<Dictionary<string, AttributeValue>>()
                            }
                        },
                    }
                };
                books.SS.ForEach(id =>
                {
                    request.RequestItems[DynamoDbConstants.BookTableName].Keys.Add(
                        new Dictionary<string, AttributeValue>
                        {
                            {
                                DynamoDbConstants.BookIdColName, new AttributeValue
                                {
                                    S = id
                                }
                            }
                        });
                });

                var response = await this._dynamoDb.BatchGetItemAsync(request);
                if (response != null && response.Responses.TryGetValue(DynamoDbConstants.BookTableName, out var bookValues))
                {
                    user.BooksRead = bookValues.Select(DynamoDbUtility.GetBookFromAttributes).ToList();
                }
            }
        }

        return users;
    }

    public async Task<InternalUser>? UpdateUser(InternalUser user)
    {
        //Unknown.jpg is the name of the default profile picture in S3
        var s3Key = "unknown.jpg";
        if (user.ProfilePicImage != null)
        {
            //Save file in S3 first to get key
            s3Key = $"ProfilePic{user.Name}";
            var imageBytes = Convert.FromBase64String(user.ProfilePicImage);
            await this._s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = this.BucketName,
                Key = s3Key,
                InputStream = new MemoryStream(imageBytes)
            });
            
        }
        else
        {
            await this.DeleteOldProfilePic(user.Name);
        }
        user.ProfilePictureS3Url = s3Key;

        var response = await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            Item = DynamoDbUtility.GetAttributesFromUser(user),
            
        });
        user.ProfilePictureS3Url = $"https://{BucketName}.s3.eu-west-2.amazonaws.com/{s3Key}";
        return user;
    }

    public async Task<InternalUser> GetUserByUsername(string username)
    {
        var result = await this._dynamoDb.QueryAsync(new QueryRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            IndexName = DynamoDbConstants.UsernameIndexColumn,
            KeyConditionExpression = $"{DynamoDbConstants.UsernameColName} = :u_username",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {
                    ":u_username", new AttributeValue
                    {
                        S = username
                    }
                }
            }
        });
        if (result == null || !result.Items.Any())
        {
            return null;
        }
        return result.Items.Select(DynamoDbUtility.GetUserFromAttributes).ToList().First();
    }

    private async Task DeleteOldProfilePic(string username)
    {
        await this._s3Client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = this.BucketName,
            Key = $"ProfilePic{username}"
        });
    }
}