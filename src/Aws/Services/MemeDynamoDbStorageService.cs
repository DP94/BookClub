using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.SharedInterfaces;
using Amazon.S3;
using Amazon.S3.Transfer;
using Aws.Util;
using Common.Util;
using Core.Models;

namespace Aws.Services;

public class MemeDynamoDbStorageService : IMemeDynamoDbStorageService
{
    
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IAmazonS3 _s3Client;

    public MemeDynamoDbStorageService(IAmazonDynamoDB dynamoDb, IAmazonS3 s3)
    {
        this._dynamoDb = dynamoDb;
        this._s3Client = s3;
    }

    public async Task<List<Meme>> GetMemes(string bookId)
    {
        var result = await this._dynamoDb.QueryAsync(new QueryRequest
        {
            TableName = DynamoDbConstants.MemeTableName,
            IndexName = DynamoDbConstants.BookIdIndexName,
            KeyConditionExpression = $"{DynamoDbConstants.BookIdColName} = :b_id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                {
                    ":b_id", new AttributeValue
                    {
                        S = bookId
                    }
                }
            }
        });
        return result.Items.Select(DynamoDbUtility.GetMemeFromAttributes).ToList();
    }

    public async Task<Meme> Create(Meme meme)
    {
        //Save file in S3
        var s3Key = Guid.NewGuid().ToString();
        var util = new TransferUtility(this._s3Client);
        await util.UploadAsync(new MemoryStream(meme.Image), "bookclubmemes", s3Key);

        //Persist to DB
        await this._dynamoDb.PutItemAsync(new PutItemRequest
        {
            TableName = DynamoDbConstants.MemeTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                {
                    DynamoDbConstants.MemeIdColName, new AttributeValue(meme.Id)
                },
                {
                    DynamoDbConstants.BookIdColName, new AttributeValue(meme.BookId)
                },
                {
                    DynamoDbConstants.MemeImageNameColName, new AttributeValue(meme.ImageName)
                },
                {
                    DynamoDbConstants.MemeImageKeyColName, new AttributeValue(s3Key)
                }
            }
        });
        meme.S3URL = $"https://bookclubmemes.s3.eu-west-2.amazonaws.com/{s3Key}";
        return meme;
    }
}