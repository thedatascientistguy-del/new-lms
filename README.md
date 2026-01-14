# Library Management System API

A .NET 6 Web API application with JWT authentication, encryption, and CRUD operations for books.

## Project Structure

```
LibraryManagementSystem/
├── src/
│   ├── LibraryManagement.API/          # Web API Layer
│   ├── LibraryManagement.Core/         # Domain Entities & Interfaces
│   └── LibraryManagement.Infrastructure/ # Data Access & Services
└── database-setup.sql                   # Database initialization script
```

## Features

- **Authentication**: JWT-based authentication with signup and login
- **Encryption**: OS-based payload encryption/decryption at middleware level
- **CRUD Operations**: Full book management (Create, Read, Delete)
- **Security**: JWT validation, user-specific data access
- **Exception Handling**: Global exception handling middleware
- **Data Access**: Dapper ORM for efficient database operations

## API Endpoints

### Authentication
- `POST /api/auth/signup` - Register new user
- `POST /api/auth/login` - Login and get JWT token

### Books (Requires Authentication)
- `GET /api/books` - Get all books for authenticated user
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Add new book
- `DELETE /api/books/{id}` - Delete book

## Setup Instructions

### 1. Database Setup

Run the SQL script to create the database:

```bash
sqlcmd -S localhost -i database-setup.sql
```

Or execute the script in SQL Server Management Studio.

### 2. Update Connection String

Edit `src/LibraryManagement.API/appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Build and Run

```bash
dotnet build
dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj
```

The API will be available at `https://localhost:7xxx` (check console output for exact port).

## Usage Examples

### 1. Signup

```bash
POST /api/auth/signup
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

Response:
```json
{
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "john_doe"
}
```

### 2. Login

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

### 3. Add Book (with JWT)

```bash
POST /api/books
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "publishedYear": 2008
}
```

### 4. Get All Books

```bash
GET /api/books
Authorization: Bearer YOUR_JWT_TOKEN
```

### 5. Get Book by ID

```bash
GET /api/books/1
Authorization: Bearer YOUR_JWT_TOKEN
```

### 6. Delete Book

```bash
DELETE /api/books/1
Authorization: Bearer YOUR_JWT_TOKEN
```

## Security Features

### JWT Validation
- Every request (except auth endpoints) validates JWT token
- Token must match the user making the request
- Tampered requests return 400 Bad Request

### Encryption
- All POST/PUT payloads are encrypted
- Decryption happens at middleware level
- Uses OS-based encryption (machine-specific entropy)

### Data Isolation
- Users can only access their own books
- UserId is extracted from JWT and validated on every request

## Configuration

### JWT Settings (appsettings.json)

```json
"Jwt": {
  "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration123456",
  "Issuer": "LibraryManagementAPI",
  "Audience": "LibraryManagementClient"
}
```

**Important**: Change the SecretKey in production!

## Technologies Used

- .NET 6
- ASP.NET Core Web API
- Dapper (Micro ORM)
- SQL Server
- JWT Bearer Authentication
- AES Encryption (OS-based)

## Error Handling

The application includes global exception handling that returns:

```json
{
  "statusCode": 500,
  "message": "An error occurred while processing your request",
  "detailed": "Exception details..."
}
```

## Notes

- Encryption is OS-based using machine-specific entropy
- Passwords are hashed using SHA256
- JWT tokens expire after 24 hours
- All timestamps are stored in UTC
