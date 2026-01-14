# Library Management System - Documentation Index

Welcome to the Library Management System documentation. This index will help you navigate through all available documentation.

## 🚀 Quick Links

- **New to the project?** Start with [QUICK-START.md](QUICK-START.md)
- **Setting up for the first time?** Read [SETUP-GUIDE.md](SETUP-GUIDE.md)
- **Want to understand the architecture?** See [ARCHITECTURE.md](ARCHITECTURE.md)
- **Looking for API examples?** Check [README.md](README.md)
- **Need a project overview?** View [PROJECT-SUMMARY.md](PROJECT-SUMMARY.md)

## 📚 Documentation Files

### Getting Started
1. **[QUICK-START.md](QUICK-START.md)** ⭐ START HERE
   - Quick setup instructions
   - Basic commands
   - API endpoint reference
   - Common troubleshooting

2. **[SETUP-GUIDE.md](SETUP-GUIDE.md)**
   - Detailed setup instructions
   - Database configuration
   - Connection string examples
   - Testing workflow
   - Production considerations

### Understanding the Project
3. **[README.md](README.md)**
   - Complete project documentation
   - Feature list
   - API usage examples
   - Security features
   - Configuration guide

4. **[PROJECT-SUMMARY.md](PROJECT-SUMMARY.md)**
   - Project overview
   - Completed features checklist
   - Technology stack
   - Database schema
   - Key highlights

5. **[ARCHITECTURE.md](ARCHITECTURE.md)**
   - System architecture diagrams
   - Request flow
   - Security layers
   - Design patterns
   - Technology stack details

### Reference
6. **[FILES-CREATED.md](FILES-CREATED.md)**
   - Complete list of all files
   - Project structure
   - File organization
   - Build status

7. **[INDEX.md](INDEX.md)** (This file)
   - Documentation navigation
   - Quick links
   - File descriptions

## 🗂️ Project Files

### Database
- **[database-setup.sql](database-setup.sql)**
  - SQL script to create database and tables
  - Run this first before starting the application

### Testing & Utilities
- **[test-api.ps1](test-api.ps1)**
  - PowerShell script to test all 6 APIs
  - Automated testing workflow
  - Update the port before running

- **[EncryptionHelper.ps1](EncryptionHelper.ps1)**
  - Helper script for encryption testing
  - Demonstrates encryption concepts

- **[LibraryManagement.postman_collection.json](LibraryManagement.postman_collection.json)**
  - Postman collection for API testing
  - Import into Postman for easy testing
  - Includes all 6 endpoints

### Configuration
- **[.gitignore](.gitignore)**
  - Git ignore rules
  - Excludes build artifacts and sensitive files

- **[LibraryManagementSystem.sln](LibraryManagementSystem.sln)**
  - Visual Studio solution file
  - Contains all 3 projects

## 📁 Source Code Structure

```
src/
├── LibraryManagement.API/          # Web API Layer
│   ├── Controllers/                # API endpoints
│   ├── Middleware/                 # Custom middleware
│   ├── Program.cs                  # Application startup
│   └── appsettings.json           # Configuration
│
├── LibraryManagement.Core/         # Domain Layer
│   ├── Entities/                   # Domain models
│   ├── DTOs/                       # Data transfer objects
│   └── Interfaces/                 # Contracts
│
└── LibraryManagement.Infrastructure/ # Data Access Layer
    ├── Repositories/               # Data access (Dapper)
    └── Services/                   # Business services
```

## 🎯 API Endpoints

| # | Method | Endpoint | Description | Documentation |
|---|--------|----------|-------------|---------------|
| 1 | POST | `/api/auth/signup` | Register user | [README.md](README.md#1-signup) |
| 2 | POST | `/api/auth/login` | Login user | [README.md](README.md#2-login) |
| 3 | GET | `/api/books` | Get all books | [README.md](README.md#4-get-all-books) |
| 4 | GET | `/api/books/{id}` | Get book by ID | [README.md](README.md#5-get-book-by-id) |
| 5 | POST | `/api/books` | Add new book | [README.md](README.md#3-add-book-with-jwt) |
| 6 | DELETE | `/api/books/{id}` | Delete book | [README.md](README.md#6-delete-book) |

## 🔐 Security Features

- **JWT Authentication** - Token-based auth with 24-hour expiration
- **OS-Based Encryption** - Machine-specific AES encryption
- **User Validation** - Tamper detection and user isolation
- **Password Hashing** - SHA256 hashing
- **Exception Handling** - Global error handling

Details: [README.md - Security Features](README.md#security-features)

## 🛠️ Technologies

- .NET 6
- ASP.NET Core Web API
- Dapper (Micro ORM)
- SQL Server
- JWT Bearer Authentication
- AES Encryption

Details: [PROJECT-SUMMARY.md - Technologies Used](PROJECT-SUMMARY.md#-technologies-used)

## 📖 How to Use This Documentation

### For First-Time Setup
1. Read [QUICK-START.md](QUICK-START.md) for basic setup
2. Follow [SETUP-GUIDE.md](SETUP-GUIDE.md) for detailed instructions
3. Run the database script: `database-setup.sql`
4. Test using `test-api.ps1` or Postman collection

### For Understanding the Code
1. Review [ARCHITECTURE.md](ARCHITECTURE.md) for system design
2. Check [PROJECT-SUMMARY.md](PROJECT-SUMMARY.md) for overview
3. Read [FILES-CREATED.md](FILES-CREATED.md) for file organization
4. Explore the source code in `src/` directory

### For API Usage
1. See [README.md](README.md) for complete API documentation
2. Import [LibraryManagement.postman_collection.json](LibraryManagement.postman_collection.json)
3. Run [test-api.ps1](test-api.ps1) for automated testing
4. Use Swagger UI at `https://localhost:PORT/swagger`

### For Troubleshooting
1. Check [QUICK-START.md - Troubleshooting](QUICK-START.md#troubleshooting)
2. Review [SETUP-GUIDE.md - Troubleshooting](SETUP-GUIDE.md#troubleshooting)
3. Verify database connection in `appsettings.json`
4. Ensure SQL Server is running

## 🎓 Learning Path

### Beginner
1. Start with [QUICK-START.md](QUICK-START.md)
2. Run the application
3. Test APIs using Swagger or PowerShell script
4. Read [README.md](README.md) for API examples

### Intermediate
1. Study [ARCHITECTURE.md](ARCHITECTURE.md)
2. Understand the middleware pipeline
3. Review repository pattern implementation
4. Explore JWT authentication flow

### Advanced
1. Analyze security implementation
2. Study encryption service
3. Review Dapper usage patterns
4. Understand clean architecture principles

## 📝 Quick Commands

```bash
# Setup database
sqlcmd -S localhost -i database-setup.sql

# Build solution
dotnet build

# Run application
dotnet run --project src/LibraryManagement.API/LibraryManagement.API.csproj

# Test APIs (PowerShell)
.\test-api.ps1

# List solution projects
dotnet sln list
```

## 🔗 External Resources

- [.NET 6 Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)
- [Dapper Documentation](https://github.com/DapperLib/Dapper)
- [JWT.io](https://jwt.io/) - JWT debugger

## 📞 Support

For issues or questions:
1. Check the troubleshooting sections in documentation
2. Review error messages in console output
3. Verify database connection and configuration
4. Ensure all NuGet packages are restored

## ✅ Checklist for New Users

- [ ] Read QUICK-START.md
- [ ] Install .NET 6 SDK
- [ ] Install SQL Server
- [ ] Run database-setup.sql
- [ ] Update connection string in appsettings.json
- [ ] Build the solution
- [ ] Run the application
- [ ] Test using Swagger or PowerShell script
- [ ] Review README.md for API details
- [ ] Explore the source code

## 🎉 You're Ready!

Once you've completed the checklist above, you're ready to start using and exploring the Library Management System. Happy coding!

---

**Last Updated:** January 2026  
**Version:** 1.0  
**Framework:** .NET 6
