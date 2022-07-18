using System.Text.Json.Serialization;

namespace Core.Models;

public class Meme
{
    public string Id { get; set; }
    public string BookId { get; set; }
    
    public byte[] Image { get; set; }
    
    public string ImageName { get; set; }
    
    public string S3URL { get; set; }
    
    public string UploadedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
}