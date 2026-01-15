# Frontend Setup Guide

## Overview
The frontend is a simple HTML/CSS/JavaScript application that connects to your Library Management API.

---

## How to Run the Frontend

### Option 1: Using Live Server (Recommended)

1. **Install Live Server Extension in VS Code**
   - Open VS Code
   - Go to Extensions (Ctrl+Shift+X)
   - Search for "Live Server"
   - Install it

2. **Open the Frontend Folder**
   - Open VS Code
   - File → Open Folder
   - Select the `frontend` folder

3. **Start Live Server**
   - Right-click on `index.html`
   - Select "Open with Live Server"
   - Browser opens at `http://127.0.0.1:5500`

### Option 2: Using Python HTTP Server

1. Open Command Prompt
2. Navigate to frontend folder:
   ```cmd
   cd C:\Users\asim.jamil\Downloads\new-lms\frontend
   ```
3. Run Python server:
   ```cmd
   python -m http.server 8000
   ```
4. Open browser: `http://localhost:8000`

### Option 3: Direct File Open (Not Recommended)

1. Navigate to the `frontend` folder
2. Double-click `index.html`
3. Opens in default browser
4. **Note:** CORS might block API calls

---

## Before Running Frontend

### 1. Make Sure Backend is Running

The frontend needs the API to be running:
- Open Visual Studio
- Run the API project (F5)
- API should be running at `https://localhost:7xxx`

### 2. Update API URL in Frontend

Open `frontend/app.js` and update the API URL:

```javascript
const API_BASE_URL = 'https://localhost:7001'; // Change port to match your API
```

**To find your API port:**
- When you run the API in Visual Studio
- Check the browser URL that opens
- Copy the port number (e.g., 7001, 7123, etc.)

### 3. Enable CORS in Backend (If needed)

If you get CORS errors, the backend already has CORS enabled in `Program.cs`.

---

## Using the Frontend

### 1. Sign Up
- Open the frontend in browser
- Click "Sign Up"
- Enter username, email, password
- Click "Sign Up" button
- You'll be logged in automatically

### 2. Login
- Enter your email and password
- Click "Login"
- You'll see the dashboard

### 3. View Books
- After login, you'll see all books
- Books are displayed in a grid

### 4. Add Book
- Click "Add New Book" button
- Fill in book details:
  - Title
  - Author
  - ISBN
  - Published Year
- Click "Save Book"

### 5. Delete Book
- Click "Delete" button on any book card
- Book will be removed

### 6. Logout
- Click "Logout" button in top right
- Returns to login page

---

## Frontend Structure

```
frontend/
├── index.html       ← Main HTML page
├── app.js          ← JavaScript logic (API calls)
├── encryption.js   ← Encryption utilities (if needed)
└── styles.css      ← Styling
```

---

## Troubleshooting

### ❌ Error: "Failed to fetch" or "Network Error"

**Cause:** Backend API is not running or wrong URL

**Solution:**
1. Make sure API is running in Visual Studio
2. Check the API URL in `app.js` matches your backend port
3. Check browser console for exact error

### ❌ Error: "CORS policy blocked"

**Cause:** CORS not enabled in backend

**Solution:**
Backend should already have CORS enabled. If not, add to `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// After app.UseHttpsRedirection();
app.UseCors("AllowAll");
```

### ❌ Error: "401 Unauthorized" on Books API

**Cause:** JWT token not being sent or expired

**Solution:**
1. Logout and login again
2. Check browser console for token
3. Token expires after 24 hours

### ❌ Frontend looks broken (no styling)

**Cause:** CSS file not loading

**Solution:**
1. Make sure all files are in the same folder
2. Use Live Server or HTTP server (not direct file open)
3. Check browser console for 404 errors

---

## Running Both Backend and Frontend

### Step-by-Step:

1. **Start Backend (Visual Studio)**
   - Open `LibraryManagementSystem.sln`
   - Press F5
   - Note the port number (e.g., https://localhost:7001)
   - Keep Visual Studio running

2. **Update Frontend API URL**
   - Open `frontend/app.js`
   - Update `API_BASE_URL` with your backend port
   - Save the file

3. **Start Frontend (VS Code)**
   - Open `frontend` folder in VS Code
   - Right-click `index.html`
   - Select "Open with Live Server"
   - Browser opens at http://127.0.0.1:5500

4. **Test the Application**
   - Sign up a new user
   - Login
   - Add some books
   - View, delete books

---

## Browser Console

To debug issues:
1. Press **F12** in browser
2. Go to **Console** tab
3. Check for errors
4. Check **Network** tab to see API calls

---

## Features

✅ User signup and login
✅ JWT authentication
✅ View all books
✅ Add new books
✅ Delete books
✅ Responsive design
✅ Error notifications
✅ Auto-logout on token expiry

---

## Notes

- Frontend stores JWT token in `localStorage`
- Token expires after 24 hours
- All API calls include the JWT token in Authorization header
- Passwords are sent securely over HTTPS
- The frontend is a Single Page Application (SPA)

---

## Next Steps

1. ✅ Run backend API
2. ✅ Update API URL in frontend
3. ✅ Run frontend with Live Server
4. ✅ Test signup/login
5. ✅ Test book operations

Enjoy your Library Management System! 📚
