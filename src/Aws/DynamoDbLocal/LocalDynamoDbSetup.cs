using System.Diagnostics;
using System.Reflection;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Common.Util;

namespace Aws.DynamoDbLocal;

public class LocalDynamoDbSetup : IDisposable
{
    private Process _process;
    private readonly string URL;

    public LocalDynamoDbSetup()
    {
        this.URL = "http://localhost:8000";
    }

    public async Task SetupDynamoDb()
    {
        this._process = this.StartDynamoProcess();
        var client = this.GetClient();
        await this.CreateBookTable(client);
        await this.CreateMemeTable(client);
        await this.CreateUserTable(client);
    }

    public void KillProcess()
    {
        this._process?.Kill();
    }

    public AmazonDynamoDBClient GetClient()
    {
        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = this.URL
        };
        var credentials = new BasicAWSCredentials("x", "x");
        return new AmazonDynamoDBClient(credentials, config);
    }

    private async Task CreateBookTable(IAmazonDynamoDB client)
    {
        await client.CreateTableAsync(new CreateTableRequest(
            DynamoDbConstants.BookTableName,
            new List<KeySchemaElement> { new(DynamoDbConstants.BookIdColName, KeyType.HASH) },
            new List<AttributeDefinition>
            {
                new(DynamoDbConstants.BookIdColName, ScalarAttributeType.S)
            },
            new ProvisionedThroughput(100, 100)));
    }

    private async Task CreateMemeTable(IAmazonDynamoDB client)
    {
        await client.CreateTableAsync(new CreateTableRequest
        {
            TableName = DynamoDbConstants.MemeTableName,
            KeySchema = new List<KeySchemaElement> { new(DynamoDbConstants.MemeIdColName, KeyType.HASH) },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new(DynamoDbConstants.MemeIdColName, ScalarAttributeType.S),
                new(DynamoDbConstants.BookIdColName, ScalarAttributeType.S)
            },
            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
            {
                new()
                {
                    IndexName = DynamoDbConstants.BookIdIndexName,
                    KeySchema = new List<KeySchemaElement>
                    {
                        new()
                        {
                            AttributeName = DynamoDbConstants.BookIdColName,
                            KeyType = KeyType.HASH
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput(100, 100),
                    Projection = new Projection
                    {
                        ProjectionType = ProjectionType.KEYS_ONLY
                    }
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput(100, 100)
        });
    }
    
    private async Task CreateUserTable(IAmazonDynamoDB client)
    {
        await client.CreateTableAsync(new CreateTableRequest
        {
            TableName = DynamoDbConstants.UserTableName,
            KeySchema = new List<KeySchemaElement> { new(DynamoDbConstants.UserIdColName, KeyType.HASH) },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new(DynamoDbConstants.UserIdColName, ScalarAttributeType.S),
                new(DynamoDbConstants.UsernameColName, ScalarAttributeType.S)
            },
            GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
            {
                new()
                {
                    IndexName = DynamoDbConstants.UsernameIndexColumn,
                    KeySchema = new List<KeySchemaElement>
                    {
                        new()
                        {
                            AttributeName = DynamoDbConstants.UsernameColName,
                            KeyType = KeyType.HASH
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput(100, 100),
                    Projection = new Projection
                    {
                        ProjectionType = ProjectionType.ALL
                    }
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput(100, 100)
        });
    }
    
    private Process StartDynamoProcess()
    {
        var dir = Path.GetDirectoryName(
            Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        var process = new Process
        {
            StartInfo = new ProcessStartInfo("java",
                "-Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -inMemory -sharedDb -port 8000")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.Combine(dir, "DynamoDbLocal")
            }
        };
        process.ErrorDataReceived += (DataReceivedEventHandler)((sender, args) => { Console.WriteLine(args.Data); });
        process.OutputDataReceived += (DataReceivedEventHandler)((sender, args) => { Console.WriteLine(args.Data); });
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return process;
    }

    public void Dispose()
    {
        _process.Dispose();
    }
}