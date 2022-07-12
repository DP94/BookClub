using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Aws.Util;
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

    public async Task<User?> GetUserById(string id)
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
                        AttributeValueList = { new AttributeValue(id)}
                    }
                }
            }
        });
        var items = queryResult.Items.FirstOrDefault();
        return items == null
            ? null
            : GetUserFromQueryResult(items);
    }

    public async Task<List<User>> GetAllUsers()
    {
        var result = await this._dynamoDb.ScanAsync(new ScanRequest(DynamoDbConstants.UserTableName));
        return result.Items.Select(DynamoDbUtility.GetUserFromAttributes).ToList();
    }

    private User GetUserFromQueryResult(Dictionary<string, AttributeValue> resultItems)
    {
        return new User()
        {
            Id = resultItems[DynamoDbConstants.UserIdColName].S,
            Username = resultItems[DynamoDbConstants.UsernameColName].S,
            Email = resultItems[DynamoDbConstants.EmailColName].S,
            Password = resultItems[DynamoDbConstants.PasswordColName].S,
            Salt = resultItems[DynamoDbConstants.SaltColName].S,
        };
    }
}