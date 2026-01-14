# Library Management System - Architecture

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                             │
│  (Browser, Postman, PowerShell, Mobile App, etc.)               │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTPS
                             │ JSON (Encrypted)
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    MIDDLEWARE PIPELINE                           │
│                                                                  │
│  1. ExceptionHandlingMiddleware                                 │
│     └─► Catches all unhandled exceptions                        │
│                                                                  │
│  2. DecryptionMiddleware                                        │
│     └─► Decrypts encrypted payloads (OS-based AES)             │
│                                                                  │
│  3. Authentication Middleware (ASP.NET Core)                    │
│     └─► Validates JWT token format                             │
│                                                                  │
│  4. Authorization Middleware (ASP.NET Core)                     │
│     └─► Checks [Authorize] attributes                          │
│                                                                  │
│  5. JwtValidationMiddleware (Custom)                            │
│     └─► Validates userId match & tamper detection              │
│                                                                  │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    API LAYER (Controllers)                       │
│                                                                  │
│  ┌──────────────────┐              ┌──────────────────┐        │
│  │ AuthController   │              │ BooksController  │        │
│  ├──────────────────┤              ├──────────────────┤        │
│  │ • Signup         │              │ • GetAll         │        │
│  │ • Login          │              │ • GetById        │        │
│  └──────────────────┘              │ • Create         │        │
│                                     │ • Delete         │        │
│                                     └──────────────────┘        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    CORE LAYER (Domain)                           │
│                                                                  │
│  ┌─────────────┐    ┌─────────────┐    ┌──────────────────┐   │
│  │  Entities   │    │    DTOs     │    │   Interfaces     │   │
│  ├─────────────┤    ├─────────────┤    ├──────────────────┤   │
│  │ • User      │    │ • Login     │    │ • IUserRepo      │   │
│  │ • Book      │    │ • Signup    │    │ • IBookRepo      │   │
│  │             │    │ • Auth      │    │ • IJwtService    │   │
│  │             │    │ • Book      │    │ • IEncryption    │   │
│  └─────────────┘    └─────────────┘    └──────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│              INFRASTRUCTURE LAYER (Data Access)                  │
│                                                                  │
│  ┌──────────────────────┐         ┌──────────────────────┐     │
│  │    Repositories      │         │      Services        │     │
│  ├──────────────────────┤         ├──────────────────────┤     │
│  │ • UserRepository     │         │ • JwtService         │     │
│  │   (Dapper)           │         │ • EncryptionService  │     │
│  │ • BookRepository     │         │   (OS-based AES)     │     │
│  │   (Dapper)           │         │                      │     │
│  └──────────────────────┘         └──────────────────────┘     │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    DATABASE LAYER                                │
│                                                                  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              SQL Server Database                          │  │
│  │                                                            │  │
│  │  ┌──────────────┐              ┌──────────────┐          │  │
│  │  │ Users Table  │              │ Books Table  │          │  │
│  │  ├──────────────┤              ├──────────────┤          │  │
│  │  │ • Id         │              │ • Id         │          │  │
│  │  │ • Username   │              │ • Title      │          │  │
│  │  │ • Email      │              │ • Author     │          │  │
│  │  │ • Password   │              │ • ISBN       │          │  │
│  │  │ • CreatedAt  │              │ • Year       │          │  │
│  │  └──────────────┘              │ • UserId (FK)│          │  │
│  │                                 │ • CreatedAt  │          │  │
│  │                                 └──────────────┘          │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## Request Flow

### Authentication Flow (Signup/Login)

```
Client
  │
  │ POST /api/auth/signup or /api/auth/login
  │ { email, password }
  ▼
ExceptionHandlingMiddleware
  │
  ▼
DecryptionMiddleware (if encrypted)
  │
  ▼
AuthController
  │
  ├─► UserRepository.GetByEmailAsync()
  │   └─► SQL Server (Dapper)
  │
  ├─► Hash Password (SHA256)
  │
  ├─► JwtService.GenerateToken()
  │   └─► Create JWT with userId & email
  │
  ▼
Response: { userId, token, username }
```

### Protected Resource Flow (Books CRUD)

```
Client
  │
  │ GET/POST/DELETE /api/books
  │ Authorization: Bearer <JWT>
  ▼
ExceptionHandlingMiddleware
  │
  ▼
DecryptionMiddleware (if POST)
  │
  ▼
Authentication Middleware
  │ Validates JWT signature
  │ Extracts claims
  ▼
Authorization Middleware
  │ Checks [Authorize] attribute
  ▼
JwtValidationMiddleware
  │ Validates userId from token
  │ Stores userId in HttpContext
  │ Checks for tampering
  ▼
BooksController
  │ Gets userId from HttpContext
  │
  ├─► BookRepository.GetAllAsync(userId)
  │   └─► SQL Server (Dapper)
  │       └─► SELECT * FROM Books WHERE UserId = @UserId
  │
  ▼
Response: Books data (user-specific)
```

## Security Layers

```
┌─────────────────────────────────────────────────────────┐
│ Layer 1: HTTPS/TLS                                      │
│ └─► Encrypts data in transit                           │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Layer 2: Payload Encryption (OS-based AES)             │
│ └─► Encrypts request body                              │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Layer 3: JWT Authentication                             │
│ └─► Validates token signature & expiration             │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Layer 4: User Validation                                │
│ └─► Validates userId match & tamper detection          │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Layer 5: Data Isolation                                 │
│ └─► SQL queries filtered by userId                     │
└─────────────────────────────────────────────────────────┘
```

## Dependency Flow

```
LibraryManagement.API
    │
    ├─► LibraryManagement.Core (Interfaces, DTOs, Entities)
    │
    └─► LibraryManagement.Infrastructure
            │
            └─► LibraryManagement.Core (Interfaces)
```

## Technology Stack

```
┌─────────────────────────────────────────────────────────┐
│ Presentation Layer                                      │
│ • ASP.NET Core Web API 6.0                             │
│ • Swagger/OpenAPI                                       │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Business Logic Layer                                    │
│ • C# 10                                                 │
│ • Dependency Injection                                  │
│ • Middleware Pipeline                                   │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Data Access Layer                                       │
│ • Dapper (Micro ORM)                                    │
│ • Repository Pattern                                    │
│ • Async/Await                                           │
└─────────────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│ Database Layer                                          │
│ • SQL Server                                            │
│ • Relational Model                                      │
└─────────────────────────────────────────────────────────┘
```

## Design Patterns Used

1. **Repository Pattern**
   - Abstracts data access logic
   - IUserRepository, IBookRepository

2. **Dependency Injection**
   - Loose coupling between layers
   - Configured in Program.cs

3. **Middleware Pattern**
   - Request/response pipeline
   - Custom middleware for encryption & validation

4. **DTO Pattern**
   - Separates domain models from API contracts
   - LoginRequest, SignupRequest, BookRequest

5. **Service Layer Pattern**
   - Business logic in services
   - JwtService, EncryptionService

6. **Clean Architecture**
   - Separation of concerns
   - Core → Infrastructure → API

## Key Components

### JWT Service
```
Responsibilities:
• Generate JWT tokens with claims
• Validate token signature
• Extract userId from token
• Handle token expiration
```

### Encryption Service
```
Responsibilities:
• Generate machine-specific keys
• Encrypt payloads (AES)
• Decrypt payloads (AES)
• Use OS-based entropy
```

### Repositories
```
Responsibilities:
• Execute SQL queries (Dapper)
• Map database results to entities
• Handle async operations
• Parameterize queries (SQL injection prevention)
```

### Middleware
```
Responsibilities:
• Exception handling (global)
• Payload decryption
• JWT validation
• User ID verification
```

## Data Flow Example: Add Book

```
1. Client sends POST request
   └─► Body: { title, author, isbn, year }
   └─► Header: Authorization: Bearer <JWT>

2. ExceptionHandlingMiddleware
   └─► Wraps request in try-catch

3. DecryptionMiddleware
   └─► Decrypts request body (if encrypted)

4. Authentication
   └─► Validates JWT signature
   └─► Extracts claims (userId, email)

5. JwtValidationMiddleware
   └─► Validates userId from token
   └─► Stores userId in HttpContext

6. BooksController.Create()
   └─► Gets userId from HttpContext
   └─► Creates Book entity
   └─► Calls BookRepository.CreateAsync()

7. BookRepository
   └─► Executes INSERT query with Dapper
   └─► Returns new book ID

8. Response
   └─► Returns created book with 201 status
```

## Scalability Considerations

### Current Implementation
- Single database connection per request
- Synchronous middleware pipeline
- In-memory JWT validation

### Future Enhancements
- Connection pooling (already in SQL Server)
- Caching layer (Redis)
- Message queue for async operations
- Load balancing
- Database replication
- API rate limiting
- Distributed caching for JWT validation

## Security Best Practices Implemented

✅ JWT token-based authentication
✅ Password hashing (SHA256)
✅ Parameterized SQL queries
✅ User data isolation
✅ HTTPS enforcement
✅ Token expiration
✅ Tamper detection
✅ OS-based encryption
✅ Exception handling (no sensitive data leaks)
✅ Dependency injection (testability)
