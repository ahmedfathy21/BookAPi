# BookAPI - Complete Project Walkthrough 📚

## Table of Contents
1. [Project Overview](#project-overview)
2. [Initial Setup](#initial-setup)
3. [Database Design](#database-design)
4. [Building the API](#building-the-api)
5. [Authentication System](#authentication-system)
6. [Testing the Application](#testing-the-application)
7. [Key Concepts Explained](#key-concepts-explained)

---

## Project Overview

**BookAPI** is a full-stack web application built with ASP.NET Core 9.0 that provides a RESTful API for managing books and authors with JWT-based authentication. The project demonstrates modern web development practices including:

- RESTful API design
- Entity Framework Core with MySQL
- JWT authentication and authorization
- One-to-many relationships
- DTO pattern for data transfer
- Swagger/OpenAPI documentation
- Blazor Server UI

---

## Initial Setup

### 1. Creating the Project

```bash
# Create new ASP.NET Core Web API project with Blazor
dotnet new webapp -o BookApi
cd BookApi
```

### 2. Installing Required Packages

```bash
# Entity Framework Core with MySQL support
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

# Swagger for API documentation
dotnet add package Swashbuckle.AspNetCore --version 9.0.6

# Authentication & Identity
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
```

### 3. Project Structure

```
BookApi/
├── Controllers/          # API endpoints
├── Models/              # Data entities
├── DTOs/                # Data Transfer Objects
├── Data/                # Database context
├── Migrations/          # Database migrations
├── Components/          # Blazor UI
└── wwwroot/            # Static files
```

---

## Database Design

### 1. Entity Models

#### **Book Model** (`Models/Book.cs`)
```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    
    // Foreign Key
    public int AuthorId { get; set; }
    
    // Navigation Property
    public Author? Author { get; set; }
}
```

**Explanation:**
- `Id`: Primary key, auto-incremented
- `Title`: Book title
- `PublishDate`: When the book was published
- `AuthorId`: Foreign key linking to the Author table
- `Author`: Navigation property for EF Core to load related author data

#### **Author Model** (`Models/Author.cs`)
```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    // Navigation Property (One-to-Many)
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
```

**Explanation:**
- `Id`: Primary key, auto-incremented
- `Name`: Author's full name
- `Bio`: Optional biography
- `DateOfBirth`: Author's birth date
- `Books`: Collection of books written by this author (one author can have many books)

#### **ApplicationUser Model** (`Models/ApplicationUser.cs`)
```csharp
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Explanation:**
- Extends `IdentityUser` which includes Email, Password (hashed), UserName, etc.
- `FullName`: Custom property for user's display name
- `CreatedAt`: Timestamp of account creation

### 2. Database Context

#### **BookContext** (`Data/BookContext.cs`)
```csharp
public class BookContext : IdentityDbContext<ApplicationUser>
{
    public BookContext(DbContextOptions<BookContext> options) 
        : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure one-to-many relationship
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Explanation:**
- Inherits from `IdentityDbContext<ApplicationUser>` to support authentication
- `DbSet<Book>` and `DbSet<Author>`: Represent database tables
- `OnModelCreating`: Configures relationships and constraints
- `DeleteBehavior.Cascade`: When an author is deleted, all their books are also deleted

### 3. Connection String Configuration

**`appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;database=BookDB;user=ahmed;password=123456"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWT12345678901234567890",
    "Issuer": "BookApi",
    "Audience": "BookApiUsers"
  }
}
```

### 4. Database Migrations

```bash
# Create initial migration for Books and Authors
dotnet ef migrations add InitialCreate
dotnet ef database update

# Later: Create migration for Identity tables
dotnet ef migrations add AddIdentityTables
dotnet ef database update
```

**What happens:**
- Creates the database schema in MySQL
- Creates tables: Books, Authors, AspNetUsers, AspNetRoles, etc.
- Sets up foreign key relationships
- Creates indexes for performance

---

## Building the API

### 1. Data Transfer Objects (DTOs)

DTOs are used to control what data is sent/received, separate from database models.

#### **CreateBookDto** (`DTOs/CreateBookDto.cs`)
```csharp
public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    // No AuthorId - set by controller from route
}
```

#### **RegisterDto** (`DTOs/RegisterDto.cs`)
```csharp
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    public string? FullName { get; set; }
}
```

**Why use DTOs?**
- Validation: Enforce data requirements
- Security: Don't expose internal model structure
- Flexibility: API can evolve independently of database

### 2. Books Controller

#### **BooksController** (`Controllers/BooksController.cs`)
```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]  // ← All endpoints require authentication
public class BooksController : ControllerBase
{
    private readonly BookContext _context;

    public BooksController(BookContext context)
    {
        _context = context;
    }

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        return await _context.Books
            .Include(b => b.Author)  // Load related author data
            .ToListAsync();
    }

    // GET: api/books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound();

        return book;
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        // Validate author exists
        var authorExists = await _context.Authors
            .AnyAsync(a => a.Id == book.AuthorId);
        
        if (!authorExists)
            return BadRequest("Author not found");

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    // PUT: api/books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
        if (id != book.Id)
            return BadRequest();

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
```

**Key Concepts:**
- `[Authorize]`: Requires JWT token for access
- `Include(b => b.Author)`: Eager loading - loads author with book
- `async/await`: Non-blocking operations for better performance
- `CreatedAtAction`: Returns 201 status with location header
- `NoContent()`: Returns 204 for successful PUT/DELETE

### 3. Authors Controller with Nested Resources

#### **AuthorsController** (`Controllers/AuthorsController.cs`)
```csharp
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly BookContext _context;

    public AuthorsController(BookContext context)
    {
        _context = context;
    }

    // POST: api/authors
    [HttpPost]
    public async Task<ActionResult<Author>> PostAuthor(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
    }

    // GET: api/authors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
    {
        return await _context.Authors
            .Include(a => a.Books)  // Load all books for each author
            .ToListAsync();
    }

    // GET: api/authors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Author>> GetAuthor(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
            return NotFound();

        return author;
    }

    // PUT: api/authors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAuthor(int id, Author author)
    {
        if (id != author.Id)
            return BadRequest();

        _context.Entry(author).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
        
        if (author == null)
            return NotFound();

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ========== NESTED BOOK ENDPOINTS ==========

    // POST: api/authors/5/books
    [HttpPost("{authorId}/books")]
    public async Task<ActionResult<Book>> CreateBookForAuthor(int authorId, CreateBookDto bookDto)
    {
        // Verify author exists
        var author = await _context.Authors.FindAsync(authorId);
        if (author == null)
            return NotFound("Author not found");

        // Create book with AuthorId from route
        var book = new Book
        {
            Title = bookDto.Title,
            PublishDate = bookDto.PublishDate,
            AuthorId = authorId  // Set from route, not DTO
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBookForAuthor), 
            new { authorId = authorId, bookId = book.Id }, book);
    }

    // GET: api/authors/5/books
    [HttpGet("{authorId}/books")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksForAuthor(int authorId)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == authorId);

        if (author == null)
            return NotFound("Author not found");

        return Ok(author.Books);
    }

    // GET: api/authors/5/books/3
    [HttpGet("{authorId}/books/{bookId}")]
    public async Task<ActionResult<Book>> GetBookForAuthor(int authorId, int bookId)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

        if (book == null)
            return NotFound();

        return book;
    }

    // PUT: api/authors/5/books/3
    [HttpPut("{authorId}/books/{bookId}")]
    public async Task<IActionResult> UpdateBookForAuthor(int authorId, int bookId, UpdateBookDto bookDto)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

        if (book == null)
            return NotFound();

        book.Title = bookDto.Title;
        book.PublishDate = bookDto.PublishDate;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(bookId))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/authors/5/books/3
    [HttpDelete("{authorId}/books/{bookId}")]
    public async Task<IActionResult> DeleteBookForAuthor(int authorId, int bookId)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

        if (book == null)
            return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AuthorExists(int id)
    {
        return _context.Authors.Any(e => e.Id == id);
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
```

**Nested Resources Explanation:**
- `/api/authors/5/books` - All books for author #5
- `/api/authors/5/books/3` - Book #3 that belongs to author #5
- This design shows the relationship clearly in the URL
- Using DTOs prevents users from changing the `AuthorId` accidentally

---

## Authentication System

### 1. Authentication Controller

#### **AuthController** (`Controllers/AuthController.cs`)
```csharp
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "User with this email already exists" });
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "User creation failed", errors = result.Errors });
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Expiration = DateTime.UtcNow.AddHours(24)
        });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Check password
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Expiration = DateTime.UtcNow.AddHours(24)
        });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(24);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

**How JWT Works:**

1. **Registration:**
   - User sends email, password, fullName
   - System creates user in database (password is hashed automatically by Identity)
   - System generates JWT token with user claims
   - Token is returned to client

2. **Login:**
   - User sends email and password
   - System verifies credentials
   - If valid, generates new JWT token
   - Token is returned to client

3. **JWT Token Structure:**
   ```
   eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.  ← Header
   eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy...  ← Payload (claims)
   SIzlISOyQOC3fzRBz-hKoParK7WCzEJAMKRJlg...  ← Signature
   ```

4. **Token Contains:**
   - User ID
   - Email
   - Full Name
   - Expiration time (24 hours)
   - Unique token ID

### 2. Program.cs Configuration

#### **Program.cs** (Authentication Setup)
```csharp
using BookApi.Components;
using BookApi.Data;
using BookApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();

// Configure Database
builder.Services.AddDbContext<BookContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings (relaxed for development)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<BookContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: Authentication must come before Authorization
app.UseAuthentication();  // ← Validates JWT token
app.UseAuthorization();   // ← Checks [Authorize] attributes

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
```

**Configuration Explained:**

1. **Database Context:**
   - Connects to MySQL database
   - Uses Pomelo MySQL provider

2. **Identity Configuration:**
   - Manages users, passwords, roles
   - Integrates with Entity Framework
   - Stores user data in AspNetUsers table

3. **JWT Authentication:**
   - Validates tokens on every request
   - Checks issuer, audience, expiration
   - Verifies signature using secret key

4. **Middleware Order:**
   ```
   Request → HTTPS Redirect → Authentication → Authorization → Controllers
   ```
   - Authentication extracts and validates JWT token
   - Authorization checks if user has permission ([Authorize] attribute)

---

## Testing the Application

### 1. Using Postman

#### **Step 1: Register a New User**
```http
POST https://localhost:7186/api/auth/register
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123",
  "fullName": "John Doe"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john@example.com",
  "expiration": "2025-12-09T00:24:44Z"
}
```

**Action:** Copy the token value!

#### **Step 2: Login (If Already Registered)**
```http
POST https://localhost:7186/api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123"
}
```

**Response:** Same as registration

#### **Step 3: Access Protected Endpoint**

**Without Token (Will Fail):**
```http
GET https://localhost:7186/api/authors
```
**Response:** `401 Unauthorized`

**With Token (Will Succeed):**
```http
GET https://localhost:7186/api/authors
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
**Response:** `200 OK` with authors data

#### **Step 4: Create an Author**
```http
POST https://localhost:7186/api/authors
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "J.K. Rowling",
  "bio": "British author best known for Harry Potter",
  "dateOfBirth": "1965-07-31T00:00:00"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "J.K. Rowling",
  "bio": "British author best known for Harry Potter",
  "dateOfBirth": "1965-07-31T00:00:00",
  "books": []
}
```

#### **Step 5: Add a Book to the Author**
```http
POST https://localhost:7186/api/authors/1/books
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "Harry Potter and the Philosopher's Stone",
  "publishDate": "1997-06-26T00:00:00"
}
```

**Response:**
```json
{
  "id": 1,
  "title": "Harry Potter and the Philosopher's Stone",
  "publishDate": "1997-06-26T00:00:00",
  "authorId": 1,
  "author": null
}
```

#### **Step 6: View All Authors with Books**
```http
GET https://localhost:7186/api/authors
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "J.K. Rowling",
    "bio": "British author best known for Harry Potter",
    "dateOfBirth": "1965-07-31T00:00:00",
    "books": [
      {
        "id": 1,
        "title": "Harry Potter and the Philosopher's Stone",
        "publishDate": "1997-06-26T00:00:00",
        "authorId": 1
      }
    ]
  }
]
```

### 2. Using cURL

```bash
# Register
curl -k -X POST https://localhost:7186/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Password123","fullName":"Test User"}'

# Login
curl -k -X POST https://localhost:7186/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Password123"}'

# Save token in variable
TOKEN="your_jwt_token_here"

# Get authors (with authentication)
curl -k -H "Authorization: Bearer $TOKEN" https://localhost:7186/api/authors
```

### 3. Using Swagger UI

1. Start the application: `dotnet run`
2. Navigate to: `https://localhost:7186/swagger`
3. Click on `/api/auth/register` → Try it out
4. Enter registration details → Execute
5. Copy the token from the response
6. Click "Authorize" button at the top
7. Enter: `Bearer your_token_here`
8. Now you can test all protected endpoints!

---

## Key Concepts Explained

### 1. RESTful API Design

**REST Principles:**
- **Resource-based URLs:** `/api/authors`, `/api/books`
- **HTTP Methods:** GET (read), POST (create), PUT (update), DELETE (delete)
- **Stateless:** Each request contains all needed information (JWT token)
- **Uniform Interface:** Consistent URL patterns and responses

**Status Codes:**
- `200 OK` - Success (GET, PUT)
- `201 Created` - Resource created (POST)
- `204 No Content` - Success with no body (DELETE)
- `400 Bad Request` - Invalid data
- `401 Unauthorized` - No or invalid token
- `404 Not Found` - Resource doesn't exist

### 2. Entity Framework Core

**What is EF Core?**
- Object-Relational Mapper (ORM)
- Converts C# objects to database tables
- Generates SQL queries automatically

**Key Features:**
- **DbContext:** Central class for database operations
- **DbSet:** Represents a table
- **Migrations:** Version control for database schema
- **LINQ Queries:** Write queries in C#, not SQL

**Example:**
```csharp
// C# LINQ Query
var books = await _context.Books
    .Include(b => b.Author)
    .Where(b => b.PublishDate.Year > 2000)
    .ToListAsync();

// Generated SQL:
// SELECT * FROM Books b
// INNER JOIN Authors a ON b.AuthorId = a.Id
// WHERE YEAR(b.PublishDate) > 2000
```

### 3. Dependency Injection

**What is DI?**
- Framework provides instances of services
- Promotes loose coupling and testability

**Example:**
```csharp
public class BooksController : ControllerBase
{
    private readonly BookContext _context;

    // Constructor injection
    public BooksController(BookContext context)
    {
        _context = context;  // Framework provides this
    }
}
```

**Configured in Program.cs:**
```csharp
builder.Services.AddDbContext<BookContext>(...);  // Registers service
```

### 4. Async/Await Pattern

**Why Async?**
- Non-blocking operations
- Better performance under load
- Frees threads while waiting for I/O (database, network)

**Example:**
```csharp
// ❌ Synchronous (blocks thread)
var books = _context.Books.ToList();

// ✅ Asynchronous (thread-friendly)
var books = await _context.Books.ToListAsync();
```

### 5. JWT Authentication Flow

```
Client                    Server
  |                         |
  |---1. POST /register---->|
  |<--2. JWT Token----------|
  |                         |
  |---3. GET /authors------>|
  |    (with token in       |
  |     Authorization       |
  |     header)             |
  |                         |
  |    Server validates     |
  |    token signature,     |
  |    expiration, etc.     |
  |                         |
  |<--4. Authors data-------|
```

**Token Validation:**
1. Extract token from `Authorization: Bearer <token>` header
2. Verify signature (using secret key)
3. Check expiration
4. Validate issuer and audience
5. Extract user claims (ID, email, etc.)
6. Allow or deny request

### 6. One-to-Many Relationships

**Database Level:**
```sql
Authors Table          Books Table
+-----------+          +-------------+
| Id (PK)   |          | Id (PK)     |
| Name      |          | Title       |
| Bio       |<------+  | AuthorId(FK)|
+-----------+       |  +-------------+
                    |
                    +--- Foreign Key
```

**C# Code Level:**
```csharp
// Author has many Books
public class Author
{
    public ICollection<Book> Books { get; set; }
}

// Book belongs to one Author
public class Book
{
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}
```

**Benefits:**
- Data integrity (can't have book with non-existent author)
- Cascade delete (delete author → delete all their books)
- Easy navigation (`book.Author.Name`)

### 7. DTO Pattern

**Why DTOs?**

**Problem:** Exposing database models directly:
```csharp
// Client sends:
{
  "id": 999,        // ← Could try to set ID
  "title": "Book",
  "authorId": 1
}
```

**Solution:** Use DTOs:
```csharp
public class CreateBookDto
{
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    // No Id, no AuthorId
}
```

**Benefits:**
- Control what data is accepted
- Validation attributes
- API can evolve independently
- Security (hide sensitive fields)

### 8. Identity Framework

**What It Provides:**
- User management (create, update, delete)
- Password hashing (bcrypt by default)
- Email confirmation
- Two-factor authentication
- Role-based authorization
- Security stamps (invalidate tokens)

**Database Tables Created:**
- `AspNetUsers` - User accounts
- `AspNetRoles` - Roles (Admin, User, etc.)
- `AspNetUserRoles` - User-Role mapping
- `AspNetUserClaims` - Additional user data
- `AspNetUserLogins` - External login providers
- `AspNetUserTokens` - Reset tokens, etc.

### 9. Middleware Pipeline

**Request Flow:**
```
Request
  ↓
UseHttpsRedirection     (Redirect HTTP → HTTPS)
  ↓
UseAuthentication       (Extract & validate JWT)
  ↓
UseAuthorization        (Check [Authorize] attributes)
  ↓
MapControllers          (Route to controller action)
  ↓
Response
```

**Order Matters!**
- Authentication must come before Authorization
- CORS before Authentication
- Exception handling at the top

---

## Summary

**What We Built:**
1. ✅ RESTful API with CRUD operations
2. ✅ MySQL database with EF Core
3. ✅ One-to-many relationships (Authors ↔ Books)
4. ✅ Nested resource endpoints
5. ✅ JWT authentication system
6. ✅ User registration and login
7. ✅ Protected API endpoints
8. ✅ DTO pattern for data validation
9. ✅ Swagger documentation
10. ✅ Blazor Server UI

**Key Technologies:**
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- ASP.NET Core Identity
- JWT Bearer Authentication
- MySQL 8.0
- Swagger/OpenAPI

**Security Features:**
- Password hashing (Identity)
- JWT token authentication
- Token expiration (24 hours)
- HTTPS enforcement
- Protected endpoints ([Authorize])

---

## Next Steps

**Enhancements You Could Add:**
1. **Role-Based Authorization** - Admin vs User roles
2. **Refresh Tokens** - Long-lived sessions
3. **Email Confirmation** - Verify user emails
4. **Pagination** - Handle large datasets
5. **Search & Filtering** - Query parameters
6. **API Versioning** - `/api/v1/`, `/api/v2/`
7. **Rate Limiting** - Prevent abuse
8. **Logging** - Serilog or NLog
9. **Unit Tests** - xUnit + Moq
10. **Docker** - Containerization

---

**🎉 Congratulations! You now understand the complete BookAPI project from start to finish!**
