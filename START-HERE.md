# 🚀 START HERE - Library Management System

Welcome! This is your starting point for the Library Management System.

## ⚡ Quick Setup (5 Minutes)

### Step 1: Setup Database
```bash
sqlcmd -S localhost -i database-setup.sql
```

### Step 2: Update Connection String
Edit `src/LibraryManagement.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### Step 3: Run the Application
```bash
dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj
```

### Step 4: Test the API
Open your browser: `https://localhost:PORT/swagger`

## ✅ What You Get

### 6 API Endpoints
1. **POST** `/api/auth/signup` - Register new user
2. **POST** `/api/auth/login` - Login and get JWT token
3. **GET** `/api/books` - Get all books
4. **GET** `/api/books/{id}` - Get book by ID
5. **POST** `/api/books` - Add new book
6. **DELETE** `/api/books/{id}` - Delete book

### Security Features
✅ JWT Authentication  
✅ OS-Based Encryption  
✅ User Data Isolation  
✅ Tamper Detection  
✅ Exception Handling  

### Architecture
✅ Clean Architecture (3 projects)  
✅ Repository Pattern  
✅ Dapper ORM  
✅ Dependency Injection  

## 📚 Documentation

Choose your path:

### 🏃 I want to get started quickly
→ Read [QUICK-START.md](QUICK-START.md)

### 📖 I want detailed setup instructions
→ Read [SETUP-GUIDE.md](SETUP-GUIDE.md)

### 🎓 I want to understand the architecture
→ Read [ARCHITECTURE.md](ARCHITECTURE.md)

### 📋 I want to see all features
→ Read [PROJECT-SUMMARY.md](PROJECT-SUMMARY.md)

### 🔍 I want API examples
→ Read [README.md](README.md)

### 🗺️ I want to navigate all docs
→ Read [INDEX.md](INDEX.md)

## 🧪 Testing Options

### Option 1: Swagger UI (Easiest)
1. Run the application
2. Open `https://localhost:PORT/swagger`
3. Test endpoints interactively

### Option 2: PowerShell Script
```powershell
# Update port in test-api.ps1 first
.\test-api.ps1
```

### Option 3: Postman
1. Import `LibraryManagement.postman_collection.json`
2. Update the `baseUrl` variable
3. Test all endpoints

## 🎯 Example Workflow

### 1. Signup
```bash
POST /api/auth/signup
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```
**Response:** `{ userId, token, username }`

### 2. Add Book (use token from signup)
```bash
POST /api/books
Authorization: Bearer YOUR_TOKEN
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "publishedYear": 2008
}
```

### 3. Get All Books
```bash
GET /api/books
Authorization: Bearer YOUR_TOKEN
```

## 🛠️ Tech Stack

- **.NET 6** - Framework
- **ASP.NET Core Web API** - API
- **Dapper** - ORM
- **SQL Server** - Database
- **JWT** - Authentication
- **AES** - Encryption

## 📁 Project Structure

```
LibraryManagementSystem/
├── src/
│   ├── LibraryManagement.API/          # Controllers, Middleware
│   ├── LibraryManagement.Core/         # Entities, DTOs, Interfaces
│   └── LibraryManagement.Infrastructure/ # Repositories, Services
├── database-setup.sql                   # Database script
├── test-api.ps1                         # Test script
└── *.md                                 # Documentation
```

## ❓ Troubleshooting

### Database connection fails?
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Run `database-setup.sql`

### Build errors?
```bash
dotnet restore
dotnet build
```

### 401 Unauthorized?
- Include token: `Authorization: Bearer YOUR_TOKEN`
- Token expires after 24 hours - login again

## 🎉 You're Ready!

The application is complete and ready to use. Choose your documentation path above and start exploring!

---

**Need Help?** Check [INDEX.md](INDEX.md) for complete documentation navigation.
