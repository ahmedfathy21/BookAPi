namespace BookApi.Models;

public class Author
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Bio { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    // Navigation property for the one-to-many relationship
    public ICollection<Book> Books { get; set; } = new List<Book>();
    
    public override string ToString() => $"{Name}";
}
