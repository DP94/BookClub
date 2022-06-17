namespace Core.Models;

public class Book
{
    public string Id { get; set; }
    public string Name { get; set; }

    public Book(string id, string name)
    {
        Id = id;
        Name = name;
    }
}