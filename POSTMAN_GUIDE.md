# Postman Collection Guide

This guide explains how to import and use the Postman collection to test the BookAPI.

## 📦 Files Included

1. **BookAPI_Postman_Collection.json** - Complete API collection with all endpoints
2. **BookAPI_Postman_Environment.json** - Environment variables for easy configuration

## 🚀 Quick Start

### Step 1: Import the Collection

1. Open Postman
2. Click **Import** button (top left)
3. Select **BookAPI_Postman_Collection.json**
4. Click **Import**

### Step 2: Import the Environment

1. Click the **Environments** tab (left sidebar)
2. Click **Import**
3. Select **BookAPI_Postman_Environment.json**
4. Click **Import**
5. Select "BookAPI Environment" from the environment dropdown (top right)

### Step 3: Start Your API

Make sure your BookAPI application is running:
```bash
dotnet run
```

The API should be available at:
- HTTP: `http://localhost:5207`
- HTTPS: `https://localhost:7186`

## 📋 Collection Structure

### 1. Authors API
- ✅ **Create Author** - POST `/api/authors`
- ✅ **Get All Authors** - GET `/api/authors`
- ✅ **Get Author By ID** - GET `/api/authors/{id}`
- ✅ **Update Author** - PUT `/api/authors/{id}`
- ✅ **Delete Author** - DELETE `/api/authors/{id}`

### 2. Author's Books API (Nested Resources)
- ✅ **Create Book for Author** - POST `/api/authors/{authorId}/books`
- ✅ **Get All Books for Author** - GET `/api/authors/{authorId}/books`
- ✅ **Get Specific Book for Author** - GET `/api/authors/{authorId}/books/{bookId}`
- ✅ **Update Book for Author** - PUT `/api/authors/{authorId}/books/{bookId}`
- ✅ **Delete Book for Author** - DELETE `/api/authors/{authorId}/books/{bookId}`

### 3. Books API
- ✅ **Get All Books** - GET `/api/books`
- ✅ **Get Book By ID** - GET `/api/books/{id}`
- ✅ **Create Book** - POST `/api/books`
- ✅ **Update Book** - PUT `/api/books/{id}`
- ✅ **Delete Book** - DELETE `/api/books/{id}`

### 4. Test Scenarios
Pre-configured test scenarios that demonstrate the complete workflow:
- Create multiple authors
- Add books to authors
- View relationships

## 🔧 Environment Variables

The collection uses these variables (automatically configured):

| Variable | Default Value | Description |
|----------|--------------|-------------|
| `baseUrl` | `http://localhost:5207` | HTTP endpoint |
| `baseUrlHttps` | `https://localhost:7186` | HTTPS endpoint |
| `authorId` | `1` | Default author ID for testing |
| `bookId` | `1` | Default book ID for testing |

### Updating Variables

After creating resources, update the IDs:
1. Click the eye icon (👁️) next to the environment dropdown
2. Edit `authorId` or `bookId` values
3. Save changes

## 📝 Testing Workflow

### Recommended Testing Order:

1. **Create an Author**
   - Use "Create Author" request
   - Note the returned `id` (e.g., `1`)

2. **Create Books for the Author**
   - Use "Create Book for Author" request
   - Update `{{authorId}}` if needed
   - Note the returned book `id`

3. **View All Authors with Books**
   - Use "Get All Authors" request
   - See the relationships

4. **Test Individual Endpoints**
   - Get specific author
   - Get books for author
   - Update book or author
   - Delete resources

### Example Test Scenario

**Scenario**: Create J.K. Rowling with Harry Potter books

1. **POST** `/api/authors`
   ```json
   {
     "name": "J.K. Rowling",
     "bio": "British author best known for the Harry Potter series",
     "dateOfBirth": "1965-07-31T00:00:00"
   }
   ```
   Response: `{ "id": 1, ... }`

2. **POST** `/api/authors/1/books`
   ```json
   {
     "title": "Harry Potter and the Philosopher's Stone",
     "publishDate": "1997-06-26T00:00:00"
   }
   ```

3. **GET** `/api/authors/1/books`
   - View all Harry Potter books

4. **GET** `/api/authors`
   - View all authors with their books

## 🎯 Using Pre-configured Test Scenarios

The collection includes a "Test Scenarios" folder with a complete workflow:

1. **Scenario 1**: Create George Orwell
2. **Scenario 2**: Add "Animal Farm" to George Orwell
3. **Scenario 3**: Create J.R.R. Tolkien
4. **Scenario 4**: Add "The Hobbit" to Tolkien
5. **Scenario 5**: Get all authors with their books

**To run all scenarios:**
1. Open the "Test Scenarios" folder
2. Run each request in order
3. Adjust IDs based on responses

## 🔍 Tips

### Using Variables in Requests
Variables are wrapped in double curly braces: `{{variableName}}`

Example:
- `{{baseUrl}}/api/authors/{{authorId}}`
- Resolves to: `http://localhost:5207/api/authors/1`

### Changing Base URL
To use HTTPS instead:
1. Edit the environment
2. Change `baseUrl` to `{{baseUrlHttps}}`
3. Or directly use: `https://localhost:7186`

### Testing Error Cases
- Try getting a non-existent author (ID: 999)
- Try creating a book without a valid authorId
- Try updating with mismatched IDs

## 📊 Expected Responses

### Success Responses

| Status | Description |
|--------|-------------|
| 200 OK | Successful GET request |
| 201 Created | Resource created successfully |
| 204 No Content | Successful PUT/DELETE request |

### Error Responses

| Status | Description |
|--------|-------------|
| 400 Bad Request | Invalid data or validation error |
| 404 Not Found | Resource not found |
| 500 Internal Server Error | Server error |

## 🛠️ Troubleshooting

### "Could not get response"
- Ensure the API is running (`dotnet run`)
- Check the correct port is being used
- Verify firewall settings

### "404 Not Found"
- Check the endpoint URL
- Verify the resource ID exists
- Make sure you've created the resource first

### "400 Bad Request"
- Check the JSON body format
- Ensure required fields are present
- Verify authorId exists when creating books

## 📚 Additional Resources

- [API Documentation](API_DOCUMENTATION.md) - Detailed endpoint documentation
- [README](README.md) - Project overview and setup
- [Swagger UI](http://localhost:5207/swagger) - Interactive API documentation (Development mode)

---

**Happy Testing!** 🚀

For issues or questions, refer to the main [README.md](README.md) file.
