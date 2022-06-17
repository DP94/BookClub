using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Common.Util;
using Core.Models;

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
        var bookList = new List<Book>();
        foreach (var i in result.Items)
        {
            if (i.TryGetValue(DynamoDbConstants.BookIdColName, out var bookId) && i.TryGetValue(DynamoDbConstants.NameColName, out var bookName))
            {
                var book = new Book(bookId.S, bookName.S);
                bookList.Add(book);
            }
        }

        return bookList;
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
        if (items.TryGetValue(DynamoDbConstants.BookIdColName, out var bookId) && items.TryGetValue(DynamoDbConstants.NameColName, out var bookName))
        {
            return new Book(bookId.S, bookName.S);
        }

        throw new ResourceNotFoundException($"Book with {id} not found!");

    }

    public async Task<Book> CreateBook(Book book)
    {
        var response = await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.BookTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {
                    DynamoDbConstants.BookIdColName, new AttributeValue(book.Id)
                },
                {
                    DynamoDbConstants.NameColName, new AttributeValue(book.Name)
                }
            }
        });

        return book;
    }
}