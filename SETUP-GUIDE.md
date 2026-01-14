# Library Management System - Setup Guide

## Prerequisites

1. **.NET 6 SDK** - Already installed (version 6.0.400)
2. **SQL Server** - Local instance or remote server
3. **SQL Server Management Studio** (optional, for database management)

## Step-by-Step Setup

### Step 1: Database Setup

1. Open SQL Server Management Studio or use `sqlcmd`
2. Execute the database setup script:

```bash
sqlcmd -S localhost -i database-setup.sql
```

Or manually run the SQL script in SSMS.

### Step 2: Configure Connection String

1. Open `src/LibraryManagement.API/appsettings.json`
2. Update the connection string to match your SQL Server instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Common connection string examples:
- Local SQL Server: `Server=localhost;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;`
- SQL Server with credentials: `Server=localhost;Database=LibraryManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;`
- Named instance: `Server=localhost\\SQLEXPRESS;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;`

### Step 3: Build the Solution

```bash
dotnet build
```

Expected output: `Build succeeded`

### Step 4: Run the Application

```bash
dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj
```

The API will start and display the URL (typically `https://localhost:7xxx`).

### Step 5: Test the API

#### Option 1: Using Swagger UI
1. Navigate to `https://localhost:7xxx/swagger` in your browser
2. Use the interactive UI to test endpoints

#### Option 2: Using PowerShell Script
1. Update the `$baseUrl` in `test-api.ps1` with your actual port
2. Run the test script:

```powershell
.\test-api.ps1
```

#### Option 3: Using cURL or Postman
See the examples in README.md

## Architecture Overview

### Project Structure

```
LibraryManagementSystem/
├── src/
│   ├── LibraryManagement.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs      # Signup & Login
│   │   │   └── BooksController.cs     # CRUD operations
│   │   ├── Middleware/
│   │   │   ├── DecryptionMiddleware.cs
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── JwtValidationMiddleware.cs
│   │   └── Program.cs
│   │
│   ├── LibraryManagement.Core/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   └── Book.cs
│   │   ├── DTOs/
│   │   │   ├── LoginRequest.cs
│   │   │   ├── SignupRequest.cs
│   │   │   ├── AuthResponse.cs
│   │   │   └── BookRequest.cs
│   │   └── Interfaces/
│   │       ├── IUserRepository.cs
│   │       ├── IBookRepository.cs
│   │       ├── IJwtService.cs
│   │       └── IEncryptionService.cs
│   │
│   └── LibraryManagement.Infrastructure/
│       ├── Repositories/
│       │   ├── UserRepository.cs      # Dapper implementation
│       │   └── BookRepository.cs      # Dapper implementation
│       └── Services/
│           ├── JwtService.cs          # JWT generation & validation
│           └── EncryptionService.cs   # OS-based encryption
```

### Middleware Pipeline

The middleware executes in this order:

1. **ExceptionHandlingMiddleware** - Catches all unhandled exceptions
2. **DecryptionMiddleware** - Decrypts encrypted payloads
3. **Authentication** - ASP.NET Core JWT authentication
4. **Authorization** - ASP.NET Core authorization
5. **JwtValidationMiddleware** - Custom JWT validation & user matching
6. **Controllers** - Your API endpoints

## Security Features Explained

### 1. JWT Authentication
- Token generated on signup/login
- Contains userId and email claims
- Expires after 24 hours
- Validated on every request (except auth endpoints)

### 2. OS-Based Encryption
- Uses machine-specific entropy (machine name + OS version)
- AES encryption with SHA256-derived keys
- Payloads are encrypted in transit
- Decryption happens at middleware level

### 3. User Data Isolation
- Each user can only access their own books
- UserId from JWT is validated against database queries
- Tampered requests are rejected with 400 Bad Request

### 4. Password Security
- Passwords are hashed using SHA256
- Never stored in plain text
- Hashed before database storage

## API Endpoints Summary

### Authentication (No token required)
- `POST /api/auth/signup` - Register new user
- `POST /api/auth/login` - Login and get JWT token

### Books (Requires JWT token in Authorization header)
- `GET /api/books` - Get all books for authenticated user
- `GET /api/books/{id}` - Get specific book by ID
- `POST /api/books` - Add new book
- `DELETE /api/books/{id}` - Delete book

## Troubleshooting

### Issue: Cannot connect to database
**Solution**: 
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database was created (run database-setup.sql)

### Issue: Build errors
**Solution**:
- Run `dotnet restore`
- Ensure all NuGet packages are installed
- Check .NET SDK version: `dotnet --version`

### Issue: 401 Unauthorized
**Solution**:
- Ensure you're including the JWT token in the Authorization header
- Format: `Authorization: Bearer YOUR_TOKEN_HERE`
- Token may have expired (24 hour lifetime)

### Issue: 400 Bad Request - Tampered request
**Solution**:
- The userId in the JWT doesn't match the request
- Get a fresh token by logging in again

## Testing Workflow

1. **Signup** - Create a new user account
2. **Login** - Get JWT token
3. **Add Book** - Create a book with the token
4. **Get All Books** - Retrieve all books for the user
5. **Get Book by ID** - Retrieve specific book
6. **Delete Book** - Remove a book

## Production Considerations

Before deploying to production:

1. **Change JWT Secret Key** in appsettings.json
2. **Use HTTPS** everywhere
3. **Implement rate limiting**
4. **Add input validation** (e.g., FluentValidation)
5. **Use stronger password hashing** (e.g., BCrypt, Argon2)
6. **Add logging** (e.g., Serilog)
7. **Implement refresh tokens** for better security
8. **Add API versioning**
9. **Configure CORS** properly
10. **Use environment variables** for sensitive configuration

## Next Steps

- Add update book endpoint (PUT)
- Implement pagination for book listing
- Add search and filtering capabilities
- Implement refresh token mechanism
- Add unit and integration tests
- Add API documentation with XML comments
- Implement role-based authorization
- Add book categories and tags
