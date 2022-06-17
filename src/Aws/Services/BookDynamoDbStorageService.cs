using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Core.Models;

namespace Aws.Services;

public class BookDynamoDbStorageService : IBookDynamoDbStorageService
{
    private IAmazonDynamoDB _dynamoDb;

    public BookDynamoDbStorageService(IAmazonDynamoDB dynamoDb)
    {
        this._dynamoDb = dynamoDb;
    }

    public List<Book> GetBooks()
    {
        throw new NotImplementedException();
    }

    public async Task<Book> GetBookById(int id)
    {
        var bookResponse = await this._dynamoDb.QueryAsync(new QueryRequest
        {
            TableName = "book",
            KeyConditions = new Dictionary<string, Condition>
            {
                {
                    "bookId",
                    new Condition
                    {
                        ComparisonOperator = ComparisonOperator.EQ,
                        AttributeValueList = { new AttributeValue(id.ToString()) }
                    }
                }
            }
        });
        var items = bookResponse.Items.FirstOrDefault();
        if (items.TryGetValue("bookId", out var bookId) && items.TryGetValue("name", out var bookName))
        {
            int.TryParse(bookId.S, out var parsedBookid);
            return new Book(parsedBookid, bookName.S);
        }

        throw new ResourceNotFoundException($"Book with {id} not found!");

    }
}