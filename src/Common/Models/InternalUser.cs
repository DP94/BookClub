namespace Common.Models;

public class InternalUser : User
{
    public string Password { get; set; }
    public string Salt { get; set; } 
}