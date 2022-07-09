
using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string? Password { get; set; }
    public string Salt { get; set; } 

    public User()
    {
        
    }
    
}