namespace Common.Util;

public static class DynamoDbConstants
{
    public const string BookTableName = "book";
    public const string MemeTableName = "meme";
    
    public const string BookIdColName = "bookId";
    public const string NameColName = "name";
    public const string ImageSourceColName = "imgSrc";
    public const string AuthorColName = "author";
    public const string SummarySourceColName = "summary";

    public const string MemeIdColName = "memeId";
    public const string MemeImageNameColName = "imageName";
    public const string MemeImageKeyColName = "s3Key";

    public const string BookIdIndexName = "BookIdIndex";
}