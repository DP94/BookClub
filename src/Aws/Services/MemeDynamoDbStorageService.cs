using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Aws.Util;
using Common.Models;
using Common.Util;
using Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Aws.Services;

public class MemeDynamoDbStorageService : IMemeDynamoDbStorageService
{
    
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly IAmazonS3 _s3Client;
    private readonly string BucketName;
    private IMemoryCache _memoryCache;
    private IUserDynamoDbStorageService _userDynamoDbStorageService;

    public MemeDynamoDbStorageService(IAmazonDynamoDB dynamoDb, IAmazonS3 s3, IMemoryCache memoryCache, IUserDynamoDbStorageService userDynamoDbStorageService)
    {
        this._dynamoDb = dynamoDb;
        this._s3Client = s3;
        this.BucketName = Environment.GetEnvironmentVariable(Constants.S3_BUCKET_NAME) ?? "";
        this._memoryCache = memoryCache;
        this._userDynamoDbStorageService = userDynamoDbStorageService;
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
        var memes = result.Items.Select(DynamoDbUtility.GetMemeFromAttributes).ToList();
        foreach (var meme in memes.Where(meme => !string.IsNullOrEmpty(meme.UploadedBy)))
        {
            //This is very likely to be similar across many memes; cache the results
            //We can't just store the username because the user may wish to change this later
            var user = await this._memoryCache.GetOrCreate(meme.UploadedBy, _ => this._userDynamoDbStorageService.GetUserById(meme.UploadedBy));
            if (user == null)
            {
                continue;
            }
            meme.UploadedBy = user.Username;
        }
        return memes;
    }

    public async Task<Meme> Create(Meme meme)
    {
        //Save file in S3 first to get key
        var s3Key = $"{Guid.NewGuid().ToString()}{meme.ImageName}";
        // await this._s3Client.PutObjectAsync(new PutObjectRequest
        // {
        //     BucketName = this.BucketName,
        //     Key = s3Key,
        //     InputStream = new MemoryStream(meme.Image)
        // });

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
                },
                {
                    DynamoDbConstants.MemeCreatedOnColName, new AttributeValue(meme.CreatedOn.ToString(CultureInfo.InvariantCulture))
                },
                {
                    DynamoDbConstants.MemeUploadedByColName, new AttributeValue(meme.UploadedBy)
                }
            }
        });
        meme.S3URL = $"https://{BucketName}.s3.eu-west-2.amazonaws.com/{s3Key}";
        meme.Image = null;
        return meme;
    }
}