# API Testing Guide - Demo Payloads

Complete guide with demo payloads for testing all 6 API endpoints.

## 🚀 Prerequisites

1. Make sure the API is running (Press F5 in Visual Studio)
2. Note the port number (e.g., `https://localhost:7123`)
3. Open Swagger UI: `https://localhost:YOUR_PORT/swagger`

---

## 📝 API Endpoint 1: Signup (Register New User)

### Request
```
POST /api/auth/signup
Content-Type: application/json
```

### Demo Payload 1 (John Doe)
```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

### Demo Payload 2 (Jane Smith)
```json
{
  "username": "jane_smith",
  "email": "jane@example.com",
  "password": "MyPassword456"
}
```

### Demo Payload 3 (Test User)
```json
{
  "username": "testuser",
  "email": "test@library.com",
  "password": "Test@123456"
}
```

### Expected Response (200 OK)
```json
{
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "john_doe"
}
```

**⚠️ IMPORTANT:** Copy the `token` value - you'll need it for all other endpoints!

---

## 🔐 API Endpoint 2: Login (Get JWT Token)

### Request
```
POST /api/auth/login
Content-Type: application/json
```

### Demo Payload 1
```json
{
  "email": "john@example.com",
  "password": "SecurePassword123"
}
```

### Demo Payload 2
```json
{
  "email": "jane@example.com",
  "password": "MyPassword456"
}
```

### Demo Payload 3
```json
{
  "email": "test@library.com",
  "password": "Test@123456"
}
```

### Expected Response (200 OK)
```json
{
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJqb2huQGV4YW1wbGUuY29tIiwiZXhwIjoxNzM3MDY4NDAwLCJpc3MiOiJMaWJyYXJ5TWFuYWdlbWVudEFQSSIsImF1ZCI6IkxpYnJhcnlNYW5hZ2VtZW50Q2xpZW50In0.xyz123...",
  "username": "john_doe"
}
```

**⚠️ IMPORTANT:** Copy the `token` value for authenticated requests!

---

## 📚 API Endpoint 3: Add Book (Create)

### Request
```
POST /api/books
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json
```

### Demo Payload 1 (Clean Code)
```json
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "publishedYear": 2008
}
```

### Demo Payload 2 (The Pragmatic Programmer)
```json
{
  "title": "The Pragmatic Programmer",
  "author": "Andrew Hunt and David Thomas",
  "isbn": "978-0201616224",
  "publishedYear": 1999
}
```

### Demo Payload 3 (Design Patterns)
```json
{
  "title": "Design Patterns: Elements of Reusable Object-Oriented Software",
  "author": "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
  "isbn": "978-0201633610",
  "publishedYear": 1994
}
```

### Demo Payload 4 (Refactoring)
```json
{
  "title": "Refactoring: Improving the Design of Existing Code",
  "author": "Martin Fowler",
  "isbn": "978-0201485677",
  "publishedYear": 1999
}
```

### Demo Payload 5 (Code Complete)
```json
{
  "title": "Code Complete",
  "author": "Steve McConnell",
  "isbn": "978-0735619678",
  "publishedYear": 2004
}
```

### Demo Payload 6 (Harry Potter)
```json
{
  "title": "Harry Potter and the Philosopher's Stone",
  "author": "J.K. Rowling",
  "isbn": "978-0747532699",
  "publishedYear": 1997
}
```

### Demo Payload 7 (1984)
```json
{
  "title": "1984",
  "author": "George Orwell",
  "isbn": "978-0451524935",
  "publishedYear": 1949
}
```

### Expected Response (201 Created)
```json
{
  "id": 1,
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "publishedYear": 2008,
  "userId": 1,
  "createdAt": "2026-01-14T18:30:00Z"
}
```

---

## 📖 API Endpoint 4: Get All Books

### Request
```
GET /api/books
Authorization: Bearer YOUR_TOKEN_HERE
```

### No Payload Required (GET request)

### Expected Response (200 OK)
```json
[
  {
    "id": 1,
    "title": "Clean Code",
    "author": "Robert C. Martin",
    "isbn": "978-0132350884",
    "publishedYear": 2008,
    "userId": 1,
    "createdAt": "2026-01-14T18:30:00Z"
  },
  {
    "id": 2,
    "title": "The Pragmatic Programmer",
    "author": "Andrew Hunt and David Thomas",
    "isbn": "978-0201616224",
    "publishedYear": 1999,
    "userId": 1,
    "createdAt": "2026-01-14T18:31:00Z"
  }
]
```

---

## 🔍 API Endpoint 5: Get Book by ID

### Request
```
GET /api/books/{id}
Authorization: Bearer YOUR_TOKEN_HERE
```

### Examples
- `GET /api/books/1` - Get book with ID 1
- `GET /api/books/2` - Get book with ID 2
- `GET /api/books/5` - Get book with ID 5

### No Payload Required (GET request)

### Expected Response (200 OK)
```json
{
  "id": 1,
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "publishedYear": 2008,
  "userId": 1,
  "createdAt": "2026-01-14T18:30:00Z"
}
```

### Expected Response (404 Not Found)
```json
"Book not found"
```

---

## 🗑️ API Endpoint 6: Delete Book

### Request
```
DELETE /api/books/{id}
Authorization: Bearer YOUR_TOKEN_HERE
```

### Examples
- `DELETE /api/books/1` - Delete book with ID 1
- `DELETE /api/books/2` - Delete book with ID 2

### No Payload Required (DELETE request)

### Expected Response (200 OK)
```json
{
  "message": "Book deleted successfully"
}
```

### Expected Response (404 Not Found)
```json
"Book not found"
```

---

## 🧪 Complete Testing Workflow

### Step 1: Create a User
```json
POST /api/auth/signup
{
  "username": "testuser",
  "email": "test@library.com",
  "password": "Test@123456"
}
```
**Save the token from response!**

### Step 2: Add Multiple Books
Use the token in Authorization header for all requests below:

**Book 1:**
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

**Book 2:**
```json
POST /api/books
Authorization: Bearer YOUR_TOKEN
{
  "title": "The Pragmatic Programmer",
  "author": "Andrew Hunt and David Thomas",
  "isbn": "978-0201616224",
  "publishedYear": 1999
}
```

**Book 3:**
```json
POST /api/books
Authorization: Bearer YOUR_TOKEN
{
  "title": "Design Patterns",
  "author": "Gang of Four",
  "isbn": "978-0201633610",
  "publishedYear": 1994
}
```

### Step 3: Get All Books
```
GET /api/books
Authorization: Bearer YOUR_TOKEN
```

### Step 4: Get Specific Book
```
GET /api/books/1
Authorization: Bearer YOUR_TOKEN
```

### Step 5: Delete a Book
```
DELETE /api/books/2
Authorization: Bearer YOUR_TOKEN
```

### Step 6: Verify Deletion
```
GET /api/books
Authorization: Bearer YOUR_TOKEN
```
(Book 2 should be gone)

---

## 🔐 Testing Security Features

### Test 1: Access Without Token (Should Fail)
```
GET /api/books
(No Authorization header)
```
**Expected:** 401 Unauthorized

### Test 2: Access With Invalid Token (Should Fail)
```
GET /api/books
Authorization: Bearer invalid_token_here
```
**Expected:** 401 Unauthorized

### Test 3: Access Another User's Book (Should Fail)
1. Create User 1 and add a book
2. Create User 2 with different credentials
3. Try to access User 1's book with User 2's token
**Expected:** 404 Not Found (data isolation)

### Test 4: Expired Token (Should Fail)
Wait 24 hours or manually test with an old token
**Expected:** 401 Unauthorized

---

## 📱 Testing with Different Tools

### Using Swagger UI (Easiest)

1. Run the application (F5 in Visual Studio)
2. Browser opens at `https://localhost:PORT/swagger`
3. Click on an endpoint (e.g., POST /api/auth/signup)
4. Click **"Try it out"**
5. Paste the demo payload
6. Click **"Execute"**
7. For authenticated endpoints:
   - Click the **"Authorize"** button at the top
   - Enter: `Bearer YOUR_TOKEN`
   - Click **"Authorize"**
   - Now all requests will include the token

### Using Postman

1. Import the collection: `LibraryManagement.postman_collection.json`
2. Update the `baseUrl` variable to your port
3. Run the requests in order
4. Token is automatically saved after login/signup

### Using PowerShell Script

```powershell
# Update the port in test-api.ps1
.\test-api.ps1
```

### Using cURL

**Signup:**
```bash
curl -X POST "https://localhost:7123/api/auth/signup" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"john_doe\",\"email\":\"john@example.com\",\"password\":\"SecurePassword123\"}"
```

**Add Book:**
```bash
curl -X POST "https://localhost:7123/api/books" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Clean Code\",\"author\":\"Robert C. Martin\",\"isbn\":\"978-0132350884\",\"publishedYear\":2008}"
```

---

## ⚠️ Common Testing Mistakes

### ❌ Mistake 1: Forgetting the Authorization Header
```
GET /api/books
(Missing: Authorization: Bearer TOKEN)
```
**Fix:** Always include the token for protected endpoints

### ❌ Mistake 2: Wrong Token Format
```
Authorization: YOUR_TOKEN
```
**Fix:** Must include "Bearer " prefix:
```
Authorization: Bearer YOUR_TOKEN
```

### ❌ Mistake 3: Using Expired Token
**Fix:** Login again to get a fresh token (24-hour expiry)

### ❌ Mistake 4: Wrong Content-Type
```
Content-Type: text/plain
```
**Fix:** Use:
```
Content-Type: application/json
```

### ❌ Mistake 5: Invalid JSON
```json
{
  title: "Clean Code"  // Missing quotes
}
```
**Fix:**
```json
{
  "title": "Clean Code"
}
```

---

## 📊 Expected HTTP Status Codes

| Endpoint | Success | Error Scenarios |
|----------|---------|-----------------|
| POST /api/auth/signup | 200 OK | 400 Bad Request (user exists) |
| POST /api/auth/login | 200 OK | 401 Unauthorized (wrong credentials) |
| POST /api/books | 201 Created | 401 Unauthorized (no token) |
| GET /api/books | 200 OK | 401 Unauthorized (no token) |
| GET /api/books/{id} | 200 OK | 404 Not Found, 401 Unauthorized |
| DELETE /api/books/{id} | 200 OK | 404 Not Found, 401 Unauthorized |

---

## 🎯 Quick Test Checklist

- [ ] Signup with new user
- [ ] Login with same credentials
- [ ] Copy the JWT token
- [ ] Add 3-5 books using the token
- [ ] Get all books
- [ ] Get specific book by ID
- [ ] Delete one book
- [ ] Verify book is deleted (get all books again)
- [ ] Try accessing without token (should fail)
- [ ] Create second user and verify data isolation

---

## 💡 Pro Tips

1. **Save Your Token:** Copy it to a text file for easy access during testing
2. **Test in Order:** Signup → Login → Add Books → Get Books → Delete
3. **Use Swagger:** It's the easiest way to test - built-in UI
4. **Check Response:** Always verify the response matches expected format
5. **Test Edge Cases:** Try invalid IDs, missing fields, wrong tokens
6. **Multiple Users:** Create 2-3 users to test data isolation

---

## 🚀 Ready to Test!

Start with Swagger UI - it's the easiest:
1. Press F5 in Visual Studio
2. Browser opens with Swagger
3. Start with POST /api/auth/signup
4. Use the demo payloads above
5. Have fun! 🎉
