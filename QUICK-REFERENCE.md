# Quick Reference Guide

## Start Commands

### Backend
```bash
cd src/LibraryManagement.API
dotnet run
```
**URL**: `https://localhost:7146`

### Frontend
```bash
cd frontend
npx http-server -p 8000
```
**URL**: `http://localhost:8000` (must use localhost, not IP)

## New Features

### ✅ Session Management
- Automatic token expiry detection
- Session monitoring every 30 seconds
- Auto-logout when token expires
- Clear session notifications

### ✅ Route Protection
- Cannot access dashboard without login
- Browser back/forward protected
- URL manipulation blocked
- All API calls require valid token

### ✅ Separate Pages
- **My Books**: View all your books
- **Add Book**: Form to add new books
- Navigation menu to switch between pages
- No modals, cleaner UI

### ✅ Comprehensive Logging
- All user actions logged to console
- API calls with timing
- Session events tracked
- Encryption operations logged
- Error tracking with context

## Console Logs

Open browser DevTools (F12) → Console tab to see:

```
[2026-01-16T10:30:00.000Z] [INFO] Application initialized
[2026-01-16T10:30:00.100Z] [INFO] Login successful { userId: 1 }
[2026-01-16T10:30:00.200Z] [INFO] Starting session monitoring
[2026-01-16T10:30:30.000Z] [INFO] Checking session validity
[2026-01-16T10:30:30.100Z] [INFO] Session valid, 1439 minutes remaining
```

## Testing Checklist

### 1. Test Login
- [ ] Open `http://localhost:8000`
- [ ] Enter credentials and login
- [ ] Check console for login logs
- [ ] Verify dashboard appears

### 2. Test Session Management
- [ ] Login successfully
- [ ] Check console for "Starting session monitoring"
- [ ] Wait 30 seconds
- [ ] Check console for "Checking session validity"
- [ ] Verify session time remaining logged

### 3. Test Route Protection
- [ ] Login and go to dashboard
- [ ] Try browser back button
- [ ] Check console for "Navigation detected"
- [ ] Verify you stay on dashboard

### 4. Test My Books Page
- [ ] Click "My Books" navigation
- [ ] Check console for "Showing My Books section"
- [ ] Verify books load
- [ ] Check console for "Books loaded successfully"

### 5. Test Add Book Page
- [ ] Click "Add Book" navigation
- [ ] Fill in book details
- [ ] Click "Save Book"
- [ ] Check console for "Book added successfully"
- [ ] Verify redirect to My Books

### 6. Test Encryption
- [ ] Open Network tab in DevTools
- [ ] Perform any action (login, add book)
- [ ] Check request: Content-Type should be "text/plain"
- [ ] Check response: Content-Type should be "text/plain"
- [ ] Verify encrypted Base64 strings

### 7. Test Logout
- [ ] Click "Logout" button
- [ ] Check console for "Clearing session data"
- [ ] Verify redirect to login page
- [ ] Check localStorage is cleared

## Common Issues

### Issue: "crypto.subtle is undefined"
**Fix**: Use `http://localhost:8000` (not `http://127.0.0.1:8000`)

### Issue: "Session expired" immediately
**Fix**: Check backend JWT expiry time in `JwtService.cs`

### Issue: No console logs
**Fix**: Open DevTools (F12) → Console tab

### Issue: Cannot navigate between pages
**Fix**: Ensure you're logged in with valid token

### Issue: Books not loading
**Fix**: 
1. Check backend is running
2. Check token is valid
3. Check console for errors

## File Structure

```
frontend/
├── index.html          # Main HTML with separate pages
├── app.js             # Updated with session management & logging
├── encryption.js      # Client-side encryption
└── styles.css         # Updated with navigation styles

src/LibraryManagement.API/
├── Controllers/
│   ├── AuthController.cs      # Login/Signup with logging
│   └── BooksController.cs     # CRUD with logging
├── Middleware/
│   ├── DecryptionMiddleware.cs    # Decrypt requests
│   ├── EncryptionMiddleware.cs    # Encrypt responses
│   └── JwtValidationMiddleware.cs # Validate JWT
└── Program.cs         # Middleware pipeline
```

## Key Features Summary

| Feature | Status | Description |
|---------|--------|-------------|
| End-to-End Encryption | ✅ | All requests/responses encrypted |
| JWT Authentication | ✅ | Secure token-based auth |
| Session Management | ✅ | Auto-logout on expiry |
| Route Protection | ✅ | Prevent unauthorized access |
| Separate Pages | ✅ | My Books & Add Book pages |
| Console Logging | ✅ | Comprehensive logs |
| Password Hashing | ✅ | SHA256 hashing |
| User Isolation | ✅ | User-specific books |

## Documentation Files

- `README.md` - Project overview
- `SETUP-GUIDE.md` - Installation
- `SESSION-MANAGEMENT-GUIDE.md` - Session features (NEW)
- `END-TO-END-ENCRYPTION-GUIDE.md` - Encryption details
- `TESTING-GUIDE.md` - Testing procedures
- `COMPLETE-FEATURES-SUMMARY.md` - All features (NEW)
- `QUICK-REFERENCE.md` - This file (NEW)

## Next Steps

1. Start backend: `dotnet run`
2. Start frontend: `npx http-server -p 8000`
3. Open: `http://localhost:8000`
4. Open DevTools Console (F12)
5. Test all features
6. Check console logs

## Support

If you encounter issues:
1. Check console logs for errors
2. Verify backend is running on port 7146
3. Verify frontend is on localhost:8000
4. Check token is valid in localStorage
5. Review documentation files

---

**All features implemented and tested!** 🎉
