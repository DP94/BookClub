namespace Core.Models;

public class Book
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    public string ImageSource { get; set; }

    public string Author { get; set; }

    public string Summary { get; set; }
    
    public Book()
    {
        
    }

    public Book(string id, string name, string imageSource, string author, string summary)
    {
        Id = id;
        Name = name;
        ImageSource = imageSource;
        Author = author;
        Summary = summary;
    }
}