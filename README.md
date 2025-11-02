# BookAPI 📚

A full-stack web application built with **ASP.NET Core 9.0** that combines a RESTful API for book management with an interactive Blazor Server UI.

## 🌟 Key Features

- **RESTful API** - Complete CRUD operations for book management
- **Blazor Server UI** - Interactive web interface with real-time updates
- **Entity Framework Core** - Modern ORM for database operations
- **MySQL Database** - Reliable data persistence
- **Swagger/OpenAPI** - Interactive API documentation
- **API Versioning Ready** - Structured for scalability

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Frontend**: Blazor Server Components
- **ORM**: Entity Framework Core 9.0
- **Database**: MySQL 8.0
- **API Documentation**: Swagger UI (Swashbuckle.AspNetCore 9.0.6)
- **Database Provider**: Pomelo.EntityFrameworkCore.MySql 9.0.0

## 📋 Project Structure

```
BookApi/
├── Controllers/
│   └── BooksController.cs          # RESTful API endpoints
├── Models/
│   └── Book.cs                     # Book entity model
├── Data/
│   └── BookContext.cs              # Entity Framework DbContext
├── Migrations/                     # EF Core database migrations
├── Components/
│   ├── Pages/                      # Blazor pages
│   └── Layout/                     # Blazor layout components
├── wwwroot/                        # Static files
├── Program.cs                      # Application entry point
└── appsettings.json               # Configuration
```

## 🚀 API Endpoints

### Books Controller (`/api/books`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | Get all books |
| GET | `/api/books/{id}` | Get a specific book by ID |
| POST | `/api/books` | Create a new book |
| PUT | `/api/books/{id}` | Update an existing book |
| DELETE | `/api/books/{id}` | Delete a book |

### Book Model

```csharp
{
    "id": 1,
    "title": "Book Title",
    "author": "Author Name",
    "publishDate": "2025-11-02T00:00:00"
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
   - **API**: `https://localhost:5001/api/books`
   - **Swagger**: `https://localhost:5001/swagger` (Development mode only)

## 📊 Database Schema

### Books Table

| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key (Auto-increment) |
| Title | VARCHAR | Book title |
| Author | VARCHAR | Book author |
| PublishDate | DATETIME | Publication date |

## 🧪 Testing the API

### Using Swagger UI (Development)
Navigate to `https://localhost:5001/swagger` to interact with the API through the browser.

### Using cURL

**Get all books:**
```bash
curl -X GET https://localhost:5001/api/books
```

**Create a new book:**
```bash
curl -X POST https://localhost:5001/api/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Clean Code",
    "author": "Robert C. Martin",
    "publishDate": "2008-08-01"
  }'
```

**Update a book:**
```bash
curl -X PUT https://localhost:5001/api/books/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "Clean Code - Updated",
    "author": "Robert C. Martin",
    "publishDate": "2008-08-01"
  }'
```

**Delete a book:**
```bash
curl -X DELETE https://localhost:5001/api/books/1
```

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

### 4. **API Documentation**
- Auto-generated Swagger documentation
- Interactive API testing interface
- OpenAPI specification

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
