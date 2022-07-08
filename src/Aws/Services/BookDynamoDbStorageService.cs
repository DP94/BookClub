using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Aws.Util;
using Common.Models;
using Common.Util;

namespace Aws.Services;

public class BookDynamoDbStorageService : IBookDynamoDbStorageService
{
    private readonly IAmazonDynamoDB _dynamoDb;

    public BookDynamoDbStorageService(IAmazonDynamoDB dynamoDb)
    {
        this._dynamoDb = dynamoDb;
    }

    public async Task<List<Book>> GetBooks()
    {
        var result = await this._dynamoDb.ScanAsync(new ScanRequest(DynamoDbConstants.BookTableName));
        return result.Items.Select(DynamoDbUtility.GetBookFromAttributes).ToList();
    }

    public async Task<Book> GetBookById(string id)
    {
        var bookResponse = await this._dynamoDb.QueryAsync(new QueryRequest
        {
            TableName = DynamoDbConstants.BookTableName,
            KeyConditions = new Dictionary<string, Condition>
            {
                {
                    DynamoDbConstants.BookIdColName,
                    new Condition
                    {
                        ComparisonOperator = ComparisonOperator.EQ,
                        AttributeValueList = { new AttributeValue(id) }
                    }
                }
            }
        });
        var items = bookResponse.Items.FirstOrDefault();
        if (items == null || !items.Any())
        {
            throw new ResourceNotFoundException($"Book with {id} not found!");
        }
        return DynamoDbUtility.GetBookFromAttributes(items);
        
    }

    public async Task<Book> CreateBook(Book book)
    {
        await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.BookTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {
                    DynamoDbConstants.BookIdColName, new AttributeValue(book.Id)
                },
                {
                    DynamoDbConstants.NameColName, new AttributeValue(book.Name)
                },
                {
                    DynamoDbConstants.ImageSourceColName, new AttributeValue(book.ImageSource)
                },
                {
                    DynamoDbConstants.SummarySourceColName, new AttributeValue(book.Summary)
                },
                {
                    DynamoDbConstants.AuthorColName, new AttributeValue(book.Author)
                }
            }
        });
        return book;
    }

    public async Task DeleteBook(string id)
    {
        var response = await this._dynamoDb.DeleteItemAsync(new DeleteItemRequest
        {
            TableName = DynamoDbConstants.BookTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                {
                    DynamoDbConstants.BookIdColName, new AttributeValue(id)
                }
            }
        });
    }

    public async Task<Book> UpdateBook(Book book)
    {
        var response = await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.BookTableName,
            Item = DynamoDbUtility.GetAttributesFromBook(book)
        });
        return book;
    }
}