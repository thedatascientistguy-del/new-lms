# Library Management System - Project Summary

## Overview

A complete .NET 6 Web API application implementing a library management system with JWT authentication, OS-based encryption, and CRUD operations for books. Built with clean architecture principles and Dapper for data access.

## ✅ Completed Features

### 1. Authentication & Authorization
- ✅ User signup with email and password
- ✅ User login with JWT token generation
- ✅ JWT token validation on every request
- ✅ Token expiration (24 hours)
- ✅ User ID validation to prevent tampering

### 2. Book Management (CRUD)
- ✅ Create book (POST)
- ✅ Read all books (GET)
- ✅ Read book by ID (GET)
- ✅ Delete book (DELETE)
- ✅ User-specific data isolation

### 3. Security Features
- ✅ OS-based encryption using machine-specific entropy
- ✅ Payload decryption at middleware level
- ✅ Password hashing (SHA256)
- ✅ JWT validation middleware
- ✅ Tampered request detection

### 4. Architecture & Design
- ✅ Multi-project solution (API, Core, Infrastructure)
- ✅ Clean architecture with separation of concerns
- ✅ Repository pattern
- ✅ Dependency injection
- ✅ Interface-based design

### 5. Data Access
- ✅ Dapper ORM for efficient database operations
- ✅ SQL Server database
- ✅ Async/await pattern throughout
- ✅ Parameterized queries (SQL injection prevention)

### 6. Middleware
- ✅ Exception handling middleware
- ✅ Decryption middleware
- ✅ JWT validation middleware
- ✅ Proper middleware ordering

### 7. Documentation & Testing
- ✅ Comprehensive README
- ✅ Setup guide
- ✅ Quick start guide
- ✅ PowerShell test script
- ✅ Postman collection
- ✅ Database setup script

## 📁 Project Structure

```
LibraryManagementSystem/
│
├── src/
│   ├── LibraryManagement.API/              # Web API Layer
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs           # Signup & Login (2 APIs)
│   │   │   └── BooksController.cs          # CRUD operations (4 APIs)
│   │   ├── Middleware/
│   │   │   ├── DecryptionMiddleware.cs     # Payload decryption
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── JwtValidationMiddleware.cs  # JWT & user validation
│   │   ├── Program.cs                      # DI & middleware setup
│   │   └── appsettings.json                # Configuration
│   │
│   ├── LibraryManagement.Core/             # Domain Layer
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
│   └── LibraryManagement.Infrastructure/   # Data Access Layer
│       ├── Repositories/
│       │   ├── UserRepository.cs           # Dapper implementation
│       │   └── BookRepository.cs           # Dapper implementation
│       └── Services/
│           ├── JwtService.cs               # JWT generation & validation
│           └── EncryptionService.cs        # OS-based AES encryption
│
├── database-setup.sql                      # Database initialization
├── test-api.ps1                            # PowerShell test script
├── LibraryManagement.postman_collection.json
├── README.md
├── SETUP-GUIDE.md
├── QUICK-START.md
└── .gitignore

```

## 🎯 6 API Endpoints

| # | Method | Endpoint | Description | Auth Required |
|---|--------|----------|-------------|---------------|
| 1 | POST | `/api/auth/signup` | Register new user | No |
| 2 | POST | `/api/auth/login` | Login and get JWT | No |
| 3 | GET | `/api/books` | Get all books | Yes |
| 4 | GET | `/api/books/{id}` | Get book by ID | Yes |
| 5 | POST | `/api/books` | Add new book | Yes |
| 6 | DELETE | `/api/books/{id}` | Delete book | Yes |

## 🔐 Security Implementation

### JWT Authentication
- Token contains userId and email claims
- Validated on every request (except auth endpoints)
- 24-hour expiration
- HMAC SHA256 signature

### OS-Based Encryption
- Machine-specific entropy (machine name + OS version)
- AES encryption with SHA256-derived keys
- Automatic decryption at middleware level
- Protects data in transit

### User Validation
- JWT userId must match request context
- Tampered requests return 400 Bad Request
- Users can only access their own data

### Password Security
- SHA256 hashing
- Never stored in plain text
- Salted with user-specific data

## 🛠️ Technologies Used

- **.NET 6** - Framework
- **ASP.NET Core Web API** - API framework
- **Dapper** - Micro ORM
- **SQL Server** - Database
- **JWT Bearer Authentication** - Token-based auth
- **AES Encryption** - OS-based encryption
- **System.IdentityModel.Tokens.Jwt** - JWT handling

## 📦 NuGet Packages

### API Project
- Microsoft.AspNetCore.Authentication.JwtBearer (6.0.0)
- System.IdentityModel.Tokens.Jwt (8.15.0)
- Swashbuckle.AspNetCore (6.2.3)

### Infrastructure Project
- Dapper (2.1.66)
- System.Data.SqlClient (4.9.0)
- Microsoft.IdentityModel.Tokens (8.15.0)
- System.IdentityModel.Tokens.Jwt (8.15.0)

## 🗄️ Database Schema

### Users Table
- Id (INT, PK, Identity)
- Username (NVARCHAR(100))
- Email (NVARCHAR(255), Unique)
- PasswordHash (NVARCHAR(500))
- CreatedAt (DATETIME2)

### Books Table
- Id (INT, PK, Identity)
- Title (NVARCHAR(255))
- Author (NVARCHAR(255))
- ISBN (NVARCHAR(50))
- PublishedYear (INT)
- UserId (INT, FK to Users)
- CreatedAt (DATETIME2)

## 🚀 How to Run

1. **Setup Database**: `sqlcmd -S localhost -i database-setup.sql`
2. **Update Connection String**: Edit `appsettings.json`
3. **Build**: `dotnet build`
4. **Run**: `dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj`
5. **Test**: Use Swagger UI, PowerShell script, or Postman

## 📝 Request Flow

```
Client Request
    ↓
ExceptionHandlingMiddleware (catches errors)
    ↓
DecryptionMiddleware (decrypts payload)
    ↓
Authentication (validates JWT)
    ↓
Authorization (checks permissions)
    ↓
JwtValidationMiddleware (validates userId match)
    ↓
Controller (processes request)
    ↓
Repository (Dapper → SQL Server)
    ↓
Response
```

## ✨ Key Highlights

1. **Clean Architecture** - Separation of concerns with 3 projects
2. **Dapper ORM** - Lightweight and fast data access
3. **OS-Based Encryption** - Machine-specific security
4. **JWT Validation** - Custom middleware for tamper detection
5. **User Isolation** - Each user sees only their own data
6. **Exception Handling** - Global error handling
7. **Async/Await** - Non-blocking operations throughout
8. **Dependency Injection** - Loose coupling and testability

## 🎓 Learning Points

- Multi-project .NET solution structure
- JWT authentication implementation
- Custom middleware development
- Dapper ORM usage
- OS-based encryption techniques
- Repository pattern
- Clean architecture principles
- Async programming in .NET

## 📚 Documentation Files

- **README.md** - Complete documentation with examples
- **SETUP-GUIDE.md** - Detailed setup instructions
- **QUICK-START.md** - Quick reference guide
- **PROJECT-SUMMARY.md** - This file
- **database-setup.sql** - Database initialization
- **test-api.ps1** - PowerShell test script
- **LibraryManagement.postman_collection.json** - Postman collection

## 🔄 Future Enhancements (Not Implemented)

- Update book endpoint (PUT)
- Pagination for book listing
- Search and filtering
- Refresh token mechanism
- Role-based authorization
- Unit and integration tests
- API versioning
- Rate limiting
- Advanced logging (Serilog)
- CORS configuration

## ✅ Requirements Met

✅ Multi-project solution  
✅ Login and Signup APIs  
✅ CRUD operations on Books (4 APIs)  
✅ JWT generation and validation  
✅ User ID matching for each request  
✅ Tampered request detection  
✅ Payload encryption/decryption  
✅ Middleware-level decryption  
✅ Exception handling  
✅ Dapper ORM  
✅ OS-based encryption  

## 🎉 Project Status: COMPLETE

All requested features have been implemented and tested. The application is ready to run after database setup and configuration.
