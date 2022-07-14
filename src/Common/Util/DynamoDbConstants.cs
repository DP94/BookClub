namespace Common.Util;

public static class DynamoDbConstants
{
    #region Book
    public const string BookTableName = "book";
    public const string MemeTableName = "meme";
    
    public const string BookIdColName = "bookId";
    public const string NameColName = "name";
    public const string ImageSourceColName = "imgSrc";
    public const string AuthorColName = "author";
    public const string SummarySourceColName = "summary";
    #endregion

    #region User
    public const string UserTableName = "user";
    public const string UserIdColName = "userId";
    public const string UsernameColName = "username";
    public const string PasswordColName = "password";
    public const string SaltColName = "salt";
    public const string EmailColName = "email";
    public const string BooksReadColumn = "booksRead";
    public const string UsernameIndexColumn = "UsernameIndex";
    #endregion
    

    public const string MemeIdColName = "memeId";
    public const string MemeImageNameColName = "imageName";
    public const string MemeImageKeyColName = "s3Key";

    public const string BookIdIndexName = "BookIdIndex";
}