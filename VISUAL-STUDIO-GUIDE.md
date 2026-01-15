# How to Run the Library Management System in Visual Studio

## Prerequisites
- ✅ Visual Studio 2022 (or 2019)
- ✅ .NET 6.0 SDK installed
- ✅ SQL Server (Express or full version)

---

## Step 1: Open the Solution

1. Open **Visual Studio**
2. Click **File** → **Open** → **Project/Solution**
3. Navigate to your project folder: `C:\Users\asim.jamil\Downloads\new-lms`
4. Select **LibraryManagementSystem.sln**
5. Click **Open**

---

## Step 2: Restore NuGet Packages

Visual Studio should automatically restore packages. If not:

1. Right-click on the **Solution** in Solution Explorer
2. Click **Restore NuGet Packages**
3. Wait for packages to download

---

## Step 3: Set Startup Project

1. In **Solution Explorer**, find `LibraryManagement.API`
2. Right-click on **LibraryManagement.API**
3. Select **Set as Startup Project**
4. The project name should now be **bold**

---

## Step 4: Configure Database Connection

1. In Solution Explorer, expand **LibraryManagement.API**
2. Open **appsettings.json**
3. Verify the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**If you're using SQL Server (not Express):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Step 5: Create Database

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Click **File** → **Open** → **File**
4. Navigate to your project folder
5. Open **database-setup.sql**
6. Click **Execute** (or press F5)
7. Verify database and tables are created

---

## Step 6: Run the Application

### Option A: Using Visual Studio (Recommended)

1. Click the **green play button** at the top (or press **F5**)
2. The button should say **LibraryManagement.API** or **https**
3. A browser window will open with Swagger UI
4. You should see: `https://localhost:7xxx/swagger/index.html`

### Option B: Using IIS Express

1. Click the dropdown next to the play button
2. Select **IIS Express**
3. Click the play button
4. Browser opens with Swagger

---

## Step 7: Test the API

Once Swagger opens, you'll see all your endpoints:

### 1. **Sign Up** (Create a user)
- Click **POST /api/auth/signup**
- Click **Try it out**
- Enter:
```json
{
  "username": "testuser",
  "email": "test@library.com",
  "password": "Test@123"
}
```
- Click **Execute**
- Copy the **token** from the response

### 2. **Authorize** (Add token)
- Click the **Authorize** button at the top (🔒 icon)
- Enter: `Bearer YOUR_TOKEN_HERE`
- Click **Authorize**
- Click **Close**

### 3. **Test Books API**
- Try **GET /api/books** - Get all books
- Try **POST /api/books** - Add a book
```json
{
  "title": "The Great Gatsby",
  "author": "F. Scott Fitzgerald",
  "isbn": "9780743273565",
  "publishedYear": 1925
}
```

---

## Troubleshooting

### ❌ Error: "Cannot connect to SQL Server"
**Solution:**
1. Open **SQL Server Configuration Manager**
2. Enable **TCP/IP** protocol
3. Restart SQL Server service
4. Or change connection string to use your server name

### ❌ Error: "Database does not exist"
**Solution:**
1. Run the **database-setup.sql** script in SSMS
2. Verify database is created

### ❌ Error: "Port already in use"
**Solution:**
1. Right-click **LibraryManagement.API** → **Properties**
2. Go to **Debug** → **General**
3. Click **Open debug launch profiles UI**
4. Change the port number in the URL
5. Save and run again

### ❌ Error: "Build failed"
**Solution:**
1. Clean the solution: **Build** → **Clean Solution**
2. Rebuild: **Build** → **Rebuild Solution**
3. Check **Error List** window for specific errors

---

## Stopping the Application

1. Close the browser window
2. In Visual Studio, click the **red square** (Stop Debugging)
3. Or press **Shift + F5**

---

## Project Structure in Visual Studio

```
Solution 'LibraryManagementSystem'
├── LibraryManagement.API          ← Startup Project (Run this)
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   └── BooksController.cs
│   ├── Middleware
│   ├── appsettings.json           ← Database connection
│   └── Program.cs
├── LibraryManagement.Core         ← Domain layer
│   ├── DTOs
│   ├── Entities
│   └── Interfaces
└── LibraryManagement.Infrastructure ← Data layer
    ├── Repositories
    └── Services
```

---

## Useful Visual Studio Shortcuts

- **F5** - Start debugging
- **Ctrl + F5** - Start without debugging (faster)
- **Shift + F5** - Stop debugging
- **Ctrl + Shift + B** - Build solution
- **Ctrl + ,** - Quick search files/classes
- **F12** - Go to definition
- **Ctrl + -** - Navigate back

---

## Next Steps

1. ✅ Run the application
2. ✅ Test signup/login
3. ✅ Test books CRUD operations
4. ✅ Check JWT token on jwt.io
5. ✅ Verify encrypted claims in token

---

## Need Help?

- Check the **Error List** window in Visual Studio (View → Error List)
- Check the **Output** window for detailed logs (View → Output)
- Check the **Console** window for runtime logs

---

**Your API will be running at:**
- Swagger UI: `https://localhost:7xxx/swagger`
- API Base URL: `https://localhost:7xxx/api`

(Port number may vary - check the browser URL when it opens)
