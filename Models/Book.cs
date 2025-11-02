namespace BookApi.Models;

public class Book
{
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public DateTime PublishDate { get; set; }
    
    // Foreign key for Author
    public int AuthorId { get; set; }
    
    // Navigation property
    public Author Author { get; set; } = null!;
    
    public override string ToString() => $"{Title} - {Author?.Name}";
}