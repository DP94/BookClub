﻿
using System.ComponentModel.DataAnnotations;

namespace Common.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    
    public string Loyalty { get; set; }
    
    public string Name { get; set; }

    public List<Book> BooksRead { get; set; }

    public User()
    {
        BooksRead = new List<Book>();
    }
    
}