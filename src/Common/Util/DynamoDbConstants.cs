namespace Common.Util;

public static class DynamoDbConstants
{
    #region Book
    public const string BookTableName = "book";
    public const string BookIdColName = "bookId";
    public const string NameColName = "name";
    public const string ImageSourceColName = "imgSrc";
    #endregion

    #region User
    public const string UserTableName = "user";
    public const string UserIdColName = "userId";
    public const string UsernameColName = "username";
    public const string PasswordColName = "password";
    public const string SaltColName = "salt";
    public const string EmailColName = "email";
    #endregion
    
}