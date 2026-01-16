# Testing Guide - End-to-End Encryption

## Quick Start

### 1. Start Backend API
```bash
cd src/LibraryManagement.API
dotnet run
```

**Expected Output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7146
```

### 2. Start Frontend
```bash
cd frontend
npx http-server -p 8000
```

**Expected Output**:
```
Available on:
  http://localhost:8000
```

### 3. Open Browser
Navigate to: `http://localhost:8000`

**Important**: Must use `localhost` (not `127.0.0.1` or IP address) for Web Crypto API to work.

## Test Scenarios

### Test 1: Signup with Encryption

1. Click "Sign Up"
2. Fill in:
   - Username: `testuser`
   - Email: `test@example.com`
   - Password: `password123`
3. Click "Sign Up"

**Expected**:
- ✅ "Signup successful!" notification
- ✅ Redirected to dashboard
- ✅ Network tab shows encrypted request (text/plain)
- ✅ Network tab shows encrypted response (text/plain)

**Verify in DevTools Network Tab**:
```
POST /api/auth/signup
Request Content-Type: text/plain
Request Payload: [Base64 encrypted string]

Response Content-Type: text/plain
Response: [Base64 encrypted string]
```

### Test 2: Login with Encryption

1. Enter credentials
2. Click "Login"

**Expected**:
- ✅ "Login successful!" notification
- ✅ Dashboard shows user email
- ✅ Request/response encrypted

### Test 3: Add Book with Encryption

1. Click "Add Book"
2. Fill in:
   - Title: `The Great Gatsby`
   - Author: `F. Scott Fitzgerald`
   - ISBN: `978-0-7432-7356-5`
   - Year: `1925`
3. Click "Save Book"

**Expected**:
- ✅ "Book added successfully!" notification
- ✅ Book appears in list
- ✅ Request encrypted
- ✅ Response encrypted

### Test 4: View Books (Encrypted Response)

1. Dashboard loads automatically
2. Books list displayed

**Expected**:
- ✅ GET /api/books returns encrypted response
- ✅ Frontend decrypts and displays books
- ✅ Only user's books shown

### Test 5: Delete Book

1. Click "Delete" on a book
2. Confirm deletion

**Expected**:
- ✅ "Book deleted successfully!" notification
- ✅ Book removed from list
- ✅ Response encrypted

### Test 6: JWT Token Verification

1. Login to get token
2. Open DevTools → Application → Local Storage
3. Copy token value
4. Go to https://jwt.io
5. Paste token

**Expected JWT Payload**:
```json
{
  "data": "8ppZywUv8ale2xdnQZITMA==+5THHR8E+wf9wWRcp5bOWgdOMLkSiEAg1YRKzLro0CQ=",
  "exp": 1768554717,
  "nbf": 1768468317,
  "iat": 1768468317
}
```

**Verify**:
- ✅ "data" claim is encrypted (Base64 string)
- ✅ Standard claims (exp, nbf, iat) are plain numbers
- ✅ JWT signature is valid

## Console Tests

### Test Encryption in Browser Console

Open browser console (F12) and run:

```javascript
// Test encryption/decryption
testEncryption();
```

**Expected Output**:
```
Original: Hello, Library!
Encrypted: [Base64 string]
Decrypted: Hello, Library!
Encryption test: ✅ PASSED
```

### Manual Encryption Test

```javascript
// Encrypt a message
const message = "Hello World";
encryptString(message).then(encrypted => {
    console.log("Encrypted:", encrypted);
    
    // Decrypt it back
    decryptString(encrypted).then(decrypted => {
        console.log("Decrypted:", decrypted);
        console.log("Match:", message === decrypted);
    });
});
```

## Backend Logs

Check backend console for comprehensive logging:

```
info: LibraryManagement.API.Controllers.AuthController[0]
      Signup attempt for email: test@example.com
info: LibraryManagement.API.Controllers.AuthController[0]
      User 1 signed up successfully with email: test@example.com

info: LibraryManagement.API.Controllers.BooksController[0]
      Fetching all books for user 1
info: LibraryManagement.API.Controllers.BooksController[0]
      Retrieved 0 books for user 1

info: LibraryManagement.API.Controllers.BooksController[0]
      Creating book 'The Great Gatsby' by F. Scott Fitzgerald for user 1
info: LibraryManagement.API.Controllers.BooksController[0]
      Book 1 created successfully for user 1
```

## Network Tab Verification

### Encrypted Request Example
```
POST https://localhost:7146/api/auth/signup
Content-Type: text/plain

YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXo=
```

### Encrypted Response Example
```
HTTP/1.1 200 OK
Content-Type: text/plain

MTIzNDU2Nzg5MGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6
```

### Decrypted in Frontend
```javascript
// Frontend automatically decrypts
{
  "userId": 1,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "testuser"
}
```

## Common Issues

### Issue: "crypto.subtle is undefined"
**Solution**: Use `http://localhost:8000` (not IP address)

### Issue: "Signup failed"
**Solution**: 
1. Check backend is running on port 7146
2. Check CORS is enabled
3. Check encryption.js is loaded

### Issue: "401 Unauthorized"
**Solution**: 
1. Login again to get fresh token
2. Check token is stored in localStorage
3. Check JWT is not expired

### Issue: Books not showing
**Solution**:
1. Check user is logged in
2. Check books belong to current user
3. Check response decryption is working

## Success Criteria

✅ All requests encrypted (text/plain Content-Type)  
✅ All responses encrypted (text/plain Content-Type)  
✅ JWT "data" claim encrypted  
✅ Frontend decrypts responses automatically  
✅ Books are user-specific  
✅ Comprehensive logging in backend  
✅ No errors in browser console  
✅ No errors in backend console  

## Performance Check

- Signup: < 500ms
- Login: < 300ms
- Get Books: < 200ms
- Add Book: < 300ms
- Delete Book: < 200ms

## Security Verification

1. **Inspect Network Traffic**: All payloads should be Base64 encrypted strings
2. **Check JWT**: User data in "data" claim should be encrypted
3. **Test User Isolation**: User A cannot see User B's books
4. **Test Token Expiry**: Expired tokens should return 401
5. **Test HTTPS**: All API calls over HTTPS

## Next Steps After Testing

If all tests pass:
1. ✅ System is fully end-to-end encrypted
2. ⏳ Add session management (auto-logout on token expiry)
3. ⏳ Add route protection (prevent URL navigation without login)
4. ⏳ Create separate page for user's books
5. ⏳ Push changes to repository

## Troubleshooting Commands

```bash
# Check if backend is running
netstat -ano | findstr :7146

# Check if frontend is running
netstat -ano | findstr :8000

# Rebuild backend
dotnet clean
dotnet build

# Clear browser cache
Ctrl + Shift + Delete

# Clear localStorage
localStorage.clear()
```
