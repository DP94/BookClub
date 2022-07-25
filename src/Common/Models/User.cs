
using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Loyalty { get; set; }
    [Required] public string Name { get; set; }
    public List<Book> BooksRead { get; set; }
    public string? ProfilePictureS3Url { get; set; }
    public string? ProfilePicImage { get; set; }

    public User()
    {
        BooksRead = new List<Book>();
    }
    
}