# API Endpoints Documentation

## Authors API

### Base URL: `/api/authors`

#### 1. Create a New Author
- **Endpoint**: `POST /api/authors`
- **Description**: Create a new author
- **Request Body**:
```json
{
  "name": "J.K. Rowling",
  "bio": "British author best known for the Harry Potter series",
  "dateOfBirth": "1965-07-31T00:00:00"
}
```
- **Response**: `201 Created`
```json
{
  "id": 1,
  "name": "J.K. Rowling",
  "bio": "British author best known for the Harry Potter series",
  "dateOfBirth": "1965-07-31T00:00:00",
  "books": []
}
```

#### 2. Get All Authors
- **Endpoint**: `GET /api/authors`
- **Description**: Retrieve a list of all authors with their books
- **Response**: `200 OK`
```json
[
  {
    "id": 1,
    "name": "J.K. Rowling",
    "bio": "British author best known for the Harry Potter series",
    "dateOfBirth": "1965-07-31T00:00:00",
    "books": [...]
  }
]
```

#### 3. Get Author by ID
- **Endpoint**: `GET /api/authors/{id}`
- **Description**: Retrieve a specific author by ID with their books
- **Response**: `200 OK` or `404 Not Found`
```json
{
  "id": 1,
  "name": "J.K. Rowling",
  "bio": "British author best known for the Harry Potter series",
  "dateOfBirth": "1965-07-31T00:00:00",
  "books": [...]
}
```

#### 4. Update Author
- **Endpoint**: `PUT /api/authors/{id}`
- **Description**: Update a specific author by ID
- **Request Body**:
```json
{
  "id": 1,
  "name": "J.K. Rowling",
  "bio": "Updated bio",
  "dateOfBirth": "1965-07-31T00:00:00"
}
```
- **Response**: `204 No Content` or `404 Not Found`

#### 5. Delete Author
- **Endpoint**: `DELETE /api/authors/{id}`
- **Description**: Delete a specific author by ID (cascades to their books)
- **Response**: `200 OK` or `404 Not Found`
```json
{
  "message": "Author deleted successfully"
}
```

---

## Author's Books API

### Base URL: `/api/authors/{authorId}/books`

#### 6. Create Book for Author
- **Endpoint**: `POST /api/authors/{authorId}/books`
- **Description**: Create a new book for a specific author
- **Request Body**:
```json
{
  "title": "Harry Potter and the Philosopher's Stone",
  "publishDate": "1997-06-26T00:00:00"
}
```
- **Response**: `201 Created`
```json
{
  "id": 1,
  "title": "Harry Potter and the Philosopher's Stone",
  "publishDate": "1997-06-26T00:00:00",
  "authorId": 1
}
```

#### 7. Get All Books for Author
- **Endpoint**: `GET /api/authors/{authorId}/books`
- **Description**: Retrieve all books for a specific author
- **Response**: `200 OK` or `404 Not Found`
```json
[
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
]
```

#### 8. Get Specific Book for Author
- **Endpoint**: `GET /api/authors/{authorId}/books/{bookId}`
- **Description**: Retrieve a specific book by ID for a specific author
- **Response**: `200 OK` or `404 Not Found`
```json
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

#### 9. Update Book for Author
- **Endpoint**: `PUT /api/authors/{authorId}/books/{bookId}`
- **Description**: Update a specific book by ID for a specific author
- **Request Body**:
```json
{
  "id": 1,
  "title": "Updated Title",
  "publishDate": "1997-06-26T00:00:00",
  "authorId": 1
}
```
- **Response**: `204 No Content`, `400 Bad Request`, or `404 Not Found`

#### 10. Delete Book for Author
- **Endpoint**: `DELETE /api/authors/{authorId}/books/{bookId}`
- **Description**: Delete a specific book by ID for a specific author
- **Response**: `200 OK` or `404 Not Found`
```json
{
  "message": "Book deleted successfully"
}
```

---

## Books API (Updated)

### Base URL: `/api/books`

**Note**: The Books API has been updated to work with the Author relationship.

#### Get All Books
- **Endpoint**: `GET /api/books`
- **Description**: Retrieve all books with author information
- **Response**: `200 OK`
```json
[
  {
    "id": 1,
    "title": "Harry Potter and the Philosopher's Stone",
    "publishDate": "1997-06-26T00:00:00",
    "authorId": 1,
    "author": {
      "id": 1,
      "name": "J.K. Rowling",
      "bio": "British author...",
      "dateOfBirth": "1965-07-31T00:00:00"
    }
  }
]
```

#### Get Book by ID
- **Endpoint**: `GET /api/books/{id}`
- **Description**: Retrieve a specific book with author information

#### Create Book
- **Endpoint**: `POST /api/books`
- **Description**: Create a new book (requires valid authorId)
- **Request Body**:
```json
{
  "title": "Book Title",
  "publishDate": "2024-01-01T00:00:00",
  "authorId": 1
}
```

#### Update Book
- **Endpoint**: `PUT /api/books/{id}`
- **Description**: Update a book (requires valid authorId)

#### Delete Book
- **Endpoint**: `DELETE /api/books/{id}`
- **Description**: Delete a book

---

## Database Schema

### Authors Table
| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key (Auto-increment) |
| Name | VARCHAR | Author's full name |
| Bio | TEXT | Author's biography |
| DateOfBirth | DATETIME | Author's date of birth |

### Books Table (Updated)
| Column | Type | Description |
|--------|------|-------------|
| Id | INT | Primary Key (Auto-increment) |
| Title | VARCHAR | Book title |
| PublishDate | DATETIME | Publication date |
| AuthorId | INT | Foreign Key to Authors table |

### Relationship
- **One-to-Many**: One Author can have many Books
- **Cascade Delete**: Deleting an author will delete all their books

---

## Testing Examples

### cURL Examples

**Create an Author:**
```bash
curl -X POST http://localhost:5207/api/authors \
  -H "Content-Type: application/json" \
  -d '{
    "name": "George Orwell",
    "bio": "English novelist and essayist",
    "dateOfBirth": "1903-06-25"
  }'
```

**Get All Authors:**
```bash
curl -X GET http://localhost:5207/api/authors
```

**Create a Book for an Author:**
```bash
curl -X POST http://localhost:5207/api/authors/1/books \
  -H "Content-Type: application/json" \
  -d '{
    "title": "1984",
    "publishDate": "1949-06-08"
  }'
```

**Get All Books for an Author:**
```bash
curl -X GET http://localhost:5207/api/authors/1/books
```

**Get a Specific Book for an Author:**
```bash
curl -X GET http://localhost:5207/api/authors/1/books/1
```

**Update a Book for an Author:**
```bash
curl -X PUT http://localhost:5207/api/authors/1/books/1 \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "1984 - Updated",
    "publishDate": "1949-06-08",
    "authorId": 1
  }'
```

**Delete a Book for an Author:**
```bash
curl -X DELETE http://localhost:5207/api/authors/1/books/1
```

---

## Swagger UI

In **Development mode**, access the interactive API documentation at:
- `https://localhost:7186/swagger`
- `http://localhost:5207/swagger`

All endpoints can be tested directly from the Swagger UI interface.
