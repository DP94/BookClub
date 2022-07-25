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
            : GetUserFromQueryResult(items);
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
        var response = await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            Item = DynamoDbUtility.GetAttributesFromUser(user),
            
        });
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

    private InternalUser GetUserFromQueryResult(Dictionary<string, AttributeValue> resultItems)
    {
        return new InternalUser
        {
            Id = resultItems[DynamoDbConstants.UserIdColName].S,
            Username = resultItems[DynamoDbConstants.UsernameColName].S,
            Email = resultItems[DynamoDbConstants.EmailColName].S,
            Password = resultItems[DynamoDbConstants.PasswordColName].S,
            Salt = resultItems[DynamoDbConstants.SaltColName].S,
            Name = resultItems[DynamoDbConstants.RealNameColumn].S,
            Loyalty = resultItems[DynamoDbConstants.LoyalistColumn].S
        };
    }
}