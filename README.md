# BookAPI 📚

A full-stack web application built with **ASP.NET Core 9.0** that combines a RESTful API for book management with an interactive Blazor Server UI. Features **JWT authentication** to secure all API endpoints.

## 🌟 Key Features

- **🔐 JWT Authentication** - Secure token-based authentication with ASP.NET Core Identity
- **RESTful API** - Complete CRUD operations for books and authors management
- **Author-Book Relationship** - One-to-many relationship with cascade delete
- **Nested Resources** - Manage books under specific authors
- **Blazor Server UI** - Interactive web interface with real-time updates
- **Entity Framework Core** - Modern ORM with navigation properties
- **MySQL Database** - Reliable data persistence with foreign key constraints
- **Swagger/OpenAPI** - Interactive API documentation
- **API Versioning Ready** - Structured for scalability

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Authentication**: ASP.NET Core Identity + JWT Bearer tokens
- **Frontend**: Blazor Server Components
- **ORM**: Entity Framework Core 9.0
- **Database**: MySQL 8.0
- **API Documentation**: Swagger UI (Swashbuckle.AspNetCore 9.0.6)
- **Database Provider**: Pomelo.EntityFrameworkCore.MySql 9.0.0
- **Security**: System.IdentityModel.Tokens.Jwt 8.0.1

## 📋 Project Structure

```
BookApi/
├── Controllers/
│   ├── BooksController.cs          # Books API endpoints (Protected)
│   ├── AuthorsController.cs        # Authors API endpoints (Protected)
│   └── AuthController.cs           # Authentication endpoints (Register/Login)
├── Models/
│   ├── Book.cs                     # Book entity model
│   ├── Author.cs                   # Author entity model
│   └── ApplicationUser.cs          # Custom user model (extends IdentityUser)
├── DTOs/
│   ├── CreateBookDto.cs            # DTO for creating books
│   ├── UpdateBookDto.cs            # DTO for updating books
│   ├── RegisterDto.cs              # DTO for user registration
│   ├── LoginDto.cs                 # DTO for user login
│   └── AuthResponseDto.cs          # DTO for authentication response
├── Data/
│   └── BookContext.cs              # Entity Framework IdentityDbContext
├── Migrations/                     # EF Core database migrations
├── Components/
│   ├── Pages/                      # Blazor pages
│   └── Layout/                     # Blazor layout components
├── wwwroot/                        # Static files
├── Program.cs                      # Application entry point (with JWT config)
├── appsettings.json               # Configuration (JWT settings)
├── README.md                       # Project documentation
├── API_DOCUMENTATION.md           # Detailed API documentation
├── BookAPI_Postman_Collection.json      # Postman collection (full)
└── BookAPI_Auth_Quick_Test.json         # Quick auth test collection
```

## � Authentication System

All API endpoints (Books and Authors) are protected and require authentication using JWT (JSON Web Tokens).

### Authentication Endpoints (`/api/auth`)

| Method | Endpoint | Description | Authentication Required |
|--------|----------|-------------|------------------------|
| POST | `/api/auth/register` | Register a new user | ❌ No |
| POST | `/api/auth/login` | Login and receive JWT token | ❌ No |

### How Authentication Works

1. **Register** a new user account
2. **Login** to receive a JWT token (valid for 24 hours)
3. **Include the token** in the `Authorization` header for all subsequent requests
4. **Token Format**: `Authorization: Bearer <your-jwt-token>`

### Example Usage

#### 1. Register a New User

```bash
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123",
  "fullName": "John Doe"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "user@example.com",
  "expiration": "2025-12-08T00:24:44Z"
}
```

#### 2. Login

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "user@example.com",
  "expiration": "2025-12-08T00:24:56Z"
}
```

#### 3. Access Protected Endpoints

```bash
GET /api/authors
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Without token:** ❌ Returns `401 Unauthorized`  
**With valid token:** ✅ Returns `200 OK` with data

## �🚀 API Endpoints

### Authors Controller (`/api/authors`) 🔒 **Protected**

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/authors` | Create a new author |
| GET | `/api/authors` | Get all authors with their books |
| GET | `/api/authors/{id}` | Get a specific author by ID |
| PUT | `/api/authors/{id}` | Update an existing author |
| DELETE | `/api/authors/{id}` | Delete an author (cascades to books) |
| POST | `/api/authors/{authorId}/books` | Create a book for a specific author |
| GET | `/api/authors/{authorId}/books` | Get all books for a specific author |
| GET | `/api/authors/{authorId}/books/{bookId}` | Get a specific book for an author |
| PUT | `/api/authors/{authorId}/books/{bookId}` | Update a book for an author |
| DELETE | `/api/authors/{authorId}/books/{bookId}` | Delete a book for an author |

> **Note:** All endpoints require a valid JWT token in the Authorization header

### Books Controller (`/api/books`) 🔒 **Protected**

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | Get all books with author information |
| GET | `/api/books/{id}` | Get a specific book by ID |
| POST | `/api/books` | Create a new book (requires authorId) |
| PUT | `/api/books/{id}` | Update an existing book |
| DELETE | `/api/books/{id}` | Delete a book |

> **Note:** All endpoints require a valid JWT token in the Authorization header

### Author Model

```csharp
{
    "id": 1,
    "name": "J.K. Rowling",
    "bio": "British author best known for the Harry Potter series",
    "dateOfBirth": "1965-07-31T00:00:00",
    "books": [...]
}
```

### Book Model

```csharp
{
    "id": 1,
    "title": "Harry Potter and the Philosopher's Stone",
    "publishDate": "1997-06-26T00:00:00",
    "authorId": 1,
    "author": {
        "id": 1,
        "name": "J.K. Rowling",
        ...
    }
}
```

## ⚙️ Setup and Installation

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- Code editor (Visual Studio, VS Code, or Rider)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/ahmedfathy21/BookAPi.git
   cd BookAPi
   ```

2. **Configure Database Connection**
   
   Update `appsettings.json` with your MySQL credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;database=BookDB;user=YOUR_USERNAME;password=YOUR_PASSWORD"
     }
   }
   ```

3. **Apply Database Migrations**
   ```bash
   dotnet ef database update
   ```
   
   *If Entity Framework tools are not installed:*
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access the Application**
   - **Blazor UI**: `https://localhost:5001` (or the port shown in terminal)
   - **API (Books)**: `https://localhost:5001/api/books`
   - **API (Authors)**: `https://localhost:5001/api/authors`
   - **Swagger**: `https://localhost:5001/swagger` (Development mode only)

## 📊 Database Schema

### Authors Table

| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key (Auto-increment) |
| Name | VARCHAR | Author's full name |
| Bio | TEXT | Author's biography |
| DateOfBirth | DATETIME | Author's date of birth |

### Books Table

| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key (Auto-increment) |
| Title | VARCHAR | Book title |
| PublishDate | DATETIME | Publication date |
| AuthorId | INT | Foreign Key to Authors table |

### Relationships

- **One-to-Many**: One Author can have many Books
- **Cascade Delete**: Deleting an author will automatically delete all their books
- **Foreign Key Constraint**: Books.AuthorId references Authors.Id

## 🧪 Testing the API

### Using Swagger UI (Development)
Navigate to `https://localhost:5001/swagger` to interact with the API through the browser.

### Using cURL

**Create an author:**
```bash
curl -X POST https://localhost:5001/api/authors \
  -H "Content-Type: application/json" \
  -d '{
    "name": "George Orwell",
    "bio": "English novelist and essayist",
    "dateOfBirth": "1903-06-25"
  }'
```

**Get all authors:**
```bash
curl -X GET https://localhost:5001/api/authors
```

**Create a book for an author:**
```bash
curl -X POST https://localhost:5001/api/authors/1/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "1984",
    "publishDate": "1949-06-08"
  }'
```

**Get all books:**
```bash
curl -X GET https://localhost:5001/api/books
```

**Get books for a specific author:**
```bash
curl -X GET https://localhost:5001/api/authors/1/books
```
  -H "Content-Type: application/json" \
**Create a book directly (with authorId):**
```bash
curl -X POST https://localhost:5001/api/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Animal Farm",
    "authorId": 1,
    "publishDate": "1945-08-17"
  }'
```

**Update a book:**
```bash
curl -X PUT https://localhost:5001/api/books/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "1984 - Updated Edition",
    "authorId": 1,
    "publishDate": "1949-06-08"
  }'
```

**Delete a book:**
```bash
curl -X DELETE https://localhost:5001/api/books/1
```

**Delete an author (and all their books):**
```bash
curl -X DELETE https://localhost:5001/api/authors/1
```

> **Note**: For complete API documentation with all endpoints and examples, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md)

## 🔧 Development

### Adding New Migrations

After modifying the `Book` model or adding new entities:

```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## 🎨 Features Breakdown

### 1. **RESTful API Architecture**
- Follows REST principles
- Proper HTTP status codes
- JSON request/response format
- Error handling and validation

### 2. **Blazor Server Integration**
- Interactive UI components
- Real-time server-side rendering
- Counter, Weather, and Home pages included
- Responsive layout with Bootstrap

### 3. **Entity Framework Core**
- Code-First approach
- Database migrations
- Async/await pattern
- LINQ query support
- Navigation properties
- One-to-many relationships
- Cascade delete support

### 4. **API Documentation**
- Auto-generated Swagger documentation
- Interactive API testing interface
- OpenAPI specification
- Comprehensive endpoint documentation

### 5. **Database Relationships**
- One-to-many relationship between Authors and Books
- Foreign key constraints
- Cascade delete operations
- Navigation properties for easy data access

## 📝 Configuration

### Environment-Specific Settings

- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development settings

### HTTPS Configuration

The application is configured to use HTTPS. Development certificates are handled automatically by .NET SDK.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 👤 Author

**Ahmed Fathy**
- GitHub: [@ahmedfathy21](https://github.com/ahmedfathy21)

## 🐛 Known Issues

- Remember to update the MySQL connection string with your credentials
- Ensure MySQL service is running before starting the application
- Swagger UI is only available in Development mode

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [MySQL Documentation](https://dev.mysql.com/doc/)

---

**Last Updated**: November 2, 2025
