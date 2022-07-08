using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Models;
using Common.Util;

namespace Aws.Services;

public class UserDynamoDbStorageService : IUserDynamoDbStorageService
{
    private readonly IAmazonDynamoDB _dynamoDb;
    
    public UserDynamoDbStorageService(IAmazonDynamoDB amazonDynamoDb)
    {
        this._dynamoDb = amazonDynamoDb;
    }

    public async Task<User> CreateUser(User user)
    {
        await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { DynamoDbConstants.UserIdColName, new AttributeValue(user.Id) },
                { DynamoDbConstants.UsernameColName, new AttributeValue(user.Username) },
                { DynamoDbConstants.EmailColName, new AttributeValue(user.Email) },
                { DynamoDbConstants.PasswordColName, new AttributeValue(user.Password) },
                { DynamoDbConstants.SaltColName, new AttributeValue(user.Salt) },
            }
        });
        return user;
    }
}