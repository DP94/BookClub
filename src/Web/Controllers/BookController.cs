using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookClub.Controllers;

[Route("v1/[controller]")]
public class BookController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        var book = new Book(1, "");
        return new string[] { "Horus Rising", "False Gods", "Galaxy in Flames", "The Flight of the Eisenstein", "Fulgrim" };
    }
    
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return $"Sanguinius{id}";
    }
    
    [HttpPost]
    public void Post([FromBody]Book book)
    {
    }
    
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]Book book)
    {
    }
    
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}