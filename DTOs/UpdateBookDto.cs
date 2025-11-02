namespace BookApi.DTOs;

public class UpdateBookDto
{
    public string Title { get; set; } = string.Empty;
    
    public DateTime PublishDate { get; set; }
}
