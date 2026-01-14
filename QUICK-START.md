# Quick Start Guide

## 1. Setup Database (One-time)

```bash
sqlcmd -S localhost -i database-setup.sql
```

## 2. Update Connection String

Edit `src/LibraryManagement.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## 3. Run the Application

```bash
dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj
```

## 4. Test the API

### Using PowerShell Script
```powershell
# Update port in test-api.ps1 first
.\test-api.ps1
```

### Using Swagger
Navigate to: `https://localhost:PORT/swagger`

### Using Postman
Import `LibraryManagement.postman_collection.json`

## API Endpoints

| Method | Endpoint | Auth Required | Description |
|--------|----------|---------------|-------------|
| POST | `/api/auth/signup` | No | Register new user |
| POST | `/api/auth/login` | No | Login and get token |
| GET | `/api/books` | Yes | Get all books |
| GET | `/api/books/{id}` | Yes | Get book by ID |
| POST | `/api/books` | Yes | Add new book |
| DELETE | `/api/books/{id}` | Yes | Delete book |

## Example Requests

### Signup
```json
POST /api/auth/signup
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

### Login
```json
POST /api/auth/login
{
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

### Add Book (with token)
```json
POST /api/books
Authorization: Bearer YOUR_TOKEN

{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "publishedYear": 2008
}
```

## Key Features

✅ JWT Authentication  
✅ OS-Based Encryption  
✅ User Data Isolation  
✅ Exception Handling  
✅ Dapper ORM  
✅ Clean Architecture  

## Troubleshooting

**Database connection fails?**
- Check SQL Server is running
- Verify connection string
- Run database-setup.sql

**401 Unauthorized?**
- Include token: `Authorization: Bearer YOUR_TOKEN`
- Token expires after 24 hours

**Build errors?**
- Run: `dotnet restore`
- Run: `dotnet build`

## Project Structure

```
src/
├── LibraryManagement.API/          # Controllers, Middleware
├── LibraryManagement.Core/         # Entities, DTOs, Interfaces
└── LibraryManagement.Infrastructure/ # Repositories, Services
```

For detailed documentation, see:
- `README.md` - Full documentation
- `SETUP-GUIDE.md` - Detailed setup instructions
