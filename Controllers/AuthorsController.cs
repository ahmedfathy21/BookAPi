using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApi.Data;
using BookApi.Models;
using BookApi.DTOs;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly BookContext _context;

        public AuthorsController(BookContext context)
        {
            _context = context;
        }

        // POST: api/authors
        [HttpPost]
        public async Task<ActionResult<Author>> CreateAuthor(Author author)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
        }

        // GET: api/authors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound(new { message = "Author not found" });

            return author;
        }

        // PUT: api/authors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
            if (id != author.Id)
                return BadRequest(new { message = "Author ID mismatch" });

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Authors.Any(e => e.Id == id))
                    return NotFound(new { message = "Author not found" });
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/authors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound(new { message = "Author not found" });

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Author deleted successfully" });
        }

        // POST: api/authors/{authorId}/books
        [HttpPost("{authorId}/books")]
        public async Task<ActionResult<Book>> CreateBookForAuthor(int authorId, CreateBookDto bookDto)
        {
            // Check if author exists
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null)
                return NotFound(new { message = "Author not found" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Create the book entity from DTO
            var book = new Book
            {
                Title = bookDto.Title,
                PublishDate = bookDto.PublishDate,
                AuthorId = authorId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBookForAuthor), 
                new { authorId = authorId, bookId = book.Id }, 
                book);
        }

        // GET: api/authors/{authorId}/books
        [HttpGet("{authorId}/books")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksForAuthor(int authorId)
        {
            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == authorId);
            if (!authorExists)
                return NotFound(new { message = "Author not found" });

            var books = await _context.Books
                .Where(b => b.AuthorId == authorId)
                .Include(b => b.Author)
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/authors/{authorId}/books/{bookId}
        [HttpGet("{authorId}/books/{bookId}")]
        public async Task<ActionResult<Book>> GetBookForAuthor(int authorId, int bookId)
        {
            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == authorId);
            if (!authorExists)
                return NotFound(new { message = "Author not found" });

            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

            if (book == null)
                return NotFound(new { message = "Book not found for this author" });

            return book;
        }

        // PUT: api/authors/{authorId}/books/{bookId}
        [HttpPut("{authorId}/books/{bookId}")]
        public async Task<IActionResult> UpdateBookForAuthor(int authorId, int bookId, UpdateBookDto bookDto)
        {
            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == authorId);
            if (!authorExists)
                return NotFound(new { message = "Author not found" });

            // Check if book exists and belongs to this author
            var existingBook = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

            if (existingBook == null)
                return NotFound(new { message = "Book not found for this author" });

            // Update the existing book
            existingBook.Title = bookDto.Title;
            existingBook.PublishDate = bookDto.PublishDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Books.Any(e => e.Id == bookId))
                    return NotFound(new { message = "Book not found" });
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/authors/{authorId}/books/{bookId}
        [HttpDelete("{authorId}/books/{bookId}")]
        public async Task<IActionResult> DeleteBookForAuthor(int authorId, int bookId)
        {
            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == authorId);
            if (!authorExists)
                return NotFound(new { message = "Author not found" });

            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

            if (book == null)
                return NotFound(new { message = "Book not found for this author" });

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book deleted successfully" });
        }
    }
}
