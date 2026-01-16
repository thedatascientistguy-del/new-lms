# Session Management & Route Protection Guide

## Overview

The Library Management System now includes comprehensive session management, route protection, and separate pages for better user experience.

## New Features Implemented

### 1. Session Management with Token Expiry

**Automatic Token Validation**:
- Token expiry is extracted from JWT and stored in localStorage
- Session validity checked every 30 seconds
- Automatic logout when token expires
- User notified before being redirected to login

**Implementation Details**:
```javascript
// Token expiry stored in localStorage
tokenExpiryTime = payload.exp * 1000; // Convert to milliseconds

// Session monitoring every 30 seconds
setInterval(() => {
    if (!isTokenValid()) {
        handleSessionExpiry();
    }
}, 30000);
```

**User Experience**:
- User logs in → Token expiry calculated and stored
- Session monitored in background
- When token expires → "Your session has expired. Please login again."
- User automatically redirected to login page
- All session data cleared

### 2. Route Protection

**Prevents Unauthorized Access**:
- Cannot navigate to dashboard without valid token
- Browser back/forward buttons protected
- URL manipulation blocked
- Hash navigation protected

**Implementation Details**:
```javascript
// Prevent back/forward navigation
window.addEventListener('popstate', (e) => {
    if (!currentToken || !isTokenValid()) {
        e.preventDefault();
        clearSession();
        showLogin();
    }
});

// Prevent URL hash manipulation
window.addEventListener('hashchange', (e) => {
    if (!currentToken || !isTokenValid()) {
        e.preventDefault();
        clearSession();
        showLogin();
    }
});
```

**Protected Actions**:
- ✅ Viewing dashboard
- ✅ Viewing books
- ✅ Adding books
- ✅ Deleting books
- ✅ All API calls

### 3. Separate Pages

**Navigation Structure**:
- **Login Page** - User authentication
- **Signup Page** - New user registration
- **Dashboard** - Main container with navigation
  - **My Books** - View all user's books
  - **Add Book** - Form to add new books

**Benefits**:
- Cleaner UI/UX
- Better organization
- Easier navigation
- No modals needed

### 4. Comprehensive Console Logging

**Log Format**:
```
[2026-01-16T10:30:45.123Z] [INFO] Application initialized
[2026-01-16T10:30:45.456Z] [INFO] Valid token found, showing dashboard
[2026-01-16T10:30:50.789Z] [INFO] API Call: GET /api/books
[2026-01-16T10:30:51.012Z] [INFO] Books loaded successfully { count: 5 }
```

**Log Levels**:
- `INFO` - Normal operations
- `WARN` - Warnings (expired tokens, unauthorized access)
- `ERROR` - Errors (API failures, encryption errors)

**Logged Events**:
- Application initialization
- User authentication (login/signup)
- Session management (token validation, expiry)
- API calls (request/response)
- Navigation (page changes)
- Book operations (add/delete/view)
- Encryption/decryption operations
- Route protection events

## Usage Guide

### Starting the Application

**Backend**:
```bash
cd src/LibraryManagement.API
dotnet run
```

**Frontend**:
```bash
cd frontend
npx http-server -p 8000
```

**Access**: `http://localhost:8000`

### Testing Session Management

**Test 1: Normal Session**
1. Login with valid credentials
2. Open browser console (F12)
3. Observe logs:
   ```
   [INFO] Login successful
   [INFO] Token expiry set { expiryDate: "2026-01-17T10:30:00.000Z" }
   [INFO] Starting session monitoring
   [INFO] Checking session validity
   [INFO] Session valid, 1439 minutes remaining
   ```

**Test 2: Token Expiry**
1. Login and wait for token to expire (24 hours by default)
2. Or manually set short expiry in backend
3. Observe automatic logout:
   ```
   [WARN] Token has expired
   [WARN] Session expired, logging out
   [INFO] Clearing session data
   [INFO] Showing login page
   ```

**Test 3: Route Protection**
1. Login and navigate to dashboard
2. Try to use browser back button to go to login
3. Try to manually change URL
4. Observe protection:
   ```
   [INFO] Navigation detected via browser history
   [WARN] Unauthorized navigation attempt blocked
   ```

### Testing Separate Pages

**Test 1: My Books Page**
1. Login to dashboard
2. Click "My Books" navigation button
3. View all your books
4. Console logs:
   ```
   [INFO] User navigated to My Books
   [INFO] Showing My Books section
   [INFO] Loading books
   [INFO] Books loaded successfully { count: 3 }
   ```

**Test 2: Add Book Page**
1. Click "Add Book" navigation button
2. Fill in book details
3. Click "Save Book"
4. Automatically redirected to "My Books"
5. Console logs:
   ```
   [INFO] User navigated to Add Book
   [INFO] Showing Add Book section
   [INFO] Save book attempt started
   [INFO] Book data collected { title: "...", author: "..." }
   [INFO] Book added successfully
   ```

## Console Logging Examples

### Successful Login Flow
```
[2026-01-16T10:30:00.000Z] [INFO] Login attempt started
[2026-01-16T10:30:00.100Z] [INFO] Login credentials collected { email: "user@example.com" }
[2026-01-16T10:30:00.200Z] [INFO] API Call: POST /api/auth/login { bodySize: 45 }
[2026-01-16T10:30:00.300Z] [INFO] Payload encrypted successfully { originalSize: 45, encryptedSize: 88 }
[2026-01-16T10:30:00.500Z] [INFO] API Response: 200 OK { duration: "300ms" }
[2026-01-16T10:30:00.600Z] [INFO] Response decrypted successfully { size: 120 }
[2026-01-16T10:30:00.700Z] [INFO] Login successful { userId: 1 }
[2026-01-16T10:30:00.800Z] [INFO] Setting session data { userId: 1, email: "user@example.com" }
[2026-01-16T10:30:00.900Z] [INFO] Token expiry set { expiryDate: "2026-01-17T10:30:00.000Z" }
[2026-01-16T10:30:01.000Z] [INFO] Showing notification { message: "Login successful!", type: "success" }
[2026-01-16T10:30:01.100Z] [INFO] Showing dashboard
[2026-01-16T10:30:01.200Z] [INFO] Starting session monitoring
```

### Session Expiry Flow
```
[2026-01-17T10:30:00.000Z] [INFO] Checking session validity
[2026-01-17T10:30:00.100Z] [WARN] Token has expired { now: 1737108600100, expiry: 1737108600000 }
[2026-01-17T10:30:00.200Z] [WARN] Session expired, logging out
[2026-01-17T10:30:00.300Z] [INFO] Handling session expiry
[2026-01-17T10:30:00.400Z] [INFO] Clearing session data
[2026-01-17T10:30:00.500Z] [INFO] Stopping session monitoring
[2026-01-17T10:30:00.600Z] [INFO] Showing login page
```

### Add Book Flow
```
[2026-01-16T10:35:00.000Z] [INFO] User navigated to Add Book
[2026-01-16T10:35:00.100Z] [INFO] Showing Add Book section
[2026-01-16T10:35:30.000Z] [INFO] Save book attempt started
[2026-01-16T10:35:30.100Z] [INFO] Book data collected { title: "1984", author: "George Orwell", isbn: "978-0451524935", publishedYear: 1949 }
[2026-01-16T10:35:30.200Z] [INFO] API Call: POST /api/books { bodySize: 95 }
[2026-01-16T10:35:30.300Z] [INFO] Authorization header added
[2026-01-16T10:35:30.400Z] [INFO] Payload encrypted successfully { originalSize: 95, encryptedSize: 176 }
[2026-01-16T10:35:30.700Z] [INFO] API Response: 201 Created { duration: "300ms" }
[2026-01-16T10:35:30.800Z] [INFO] Response decrypted successfully { size: 150 }
[2026-01-16T10:35:30.900Z] [INFO] Book added successfully { bookId: 5 }
[2026-01-16T10:35:31.000Z] [INFO] User navigated to My Books
[2026-01-16T10:35:31.100Z] [INFO] Loading books
```

### Route Protection Flow
```
[2026-01-16T10:40:00.000Z] [INFO] Navigation detected via browser history
[2026-01-16T10:40:00.100Z] [WARN] No token found
[2026-01-16T10:40:00.200Z] [WARN] Unauthorized navigation attempt blocked
[2026-01-16T10:40:00.300Z] [INFO] Clearing session data
[2026-01-16T10:40:00.400Z] [INFO] Showing login page
```

## Security Features

### Session Security
- ✅ Token expiry validation
- ✅ Automatic session cleanup
- ✅ Secure token storage (localStorage)
- ✅ Session monitoring (30-second intervals)
- ✅ Immediate logout on expiry

### Route Security
- ✅ Protected dashboard access
- ✅ Protected API calls
- ✅ Browser navigation protection
- ✅ URL manipulation prevention
- ✅ Unauthorized access blocked

### Data Security
- ✅ End-to-end encryption
- ✅ JWT claim encryption
- ✅ Password hashing
- ✅ HTTPS transport
- ✅ User data isolation

## Configuration

### Token Expiry Time
Default: 24 hours (configured in backend)

To change, modify `JwtService.cs`:
```csharp
Expires = DateTime.UtcNow.AddHours(24) // Change to desired duration
```

### Session Check Interval
Default: 30 seconds

To change, modify `app.js`:
```javascript
sessionCheckInterval = setInterval(() => {
    // Check session
}, 30000); // Change to desired interval in milliseconds
```

## Troubleshooting

### Issue: Session expires too quickly
**Solution**: Check backend JWT expiry configuration in `JwtService.cs`

### Issue: Console logs not showing
**Solution**: Open browser DevTools (F12) → Console tab

### Issue: Route protection not working
**Solution**: 
1. Check token is stored in localStorage
2. Verify token expiry time is valid
3. Check console for error messages

### Issue: Cannot navigate between pages
**Solution**: Ensure you're logged in with valid token

## Benefits

### For Users
- Clear session expiry notifications
- Automatic logout for security
- Better navigation experience
- Organized page structure
- Real-time feedback

### For Developers
- Comprehensive logging for debugging
- Easy to trace user actions
- Security event monitoring
- Performance tracking
- Error identification

## Next Steps

✅ Session management implemented  
✅ Route protection implemented  
✅ Separate pages implemented  
✅ Comprehensive logging implemented  

**Future Enhancements**:
- Remember me functionality
- Session timeout warning (5 minutes before expiry)
- Multiple device session management
- Activity-based session extension
- Logout from all devices
