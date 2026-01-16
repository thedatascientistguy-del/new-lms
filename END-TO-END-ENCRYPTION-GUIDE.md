# End-to-End Encryption Guide

## Overview

This Library Management System implements **complete end-to-end encryption** for all data transmission between client and server.

## Encryption Architecture

### 1. **Request Payload Encryption** (Client → Server)
- **Technology**: AES-CBC with shared secret key
- **Flow**: 
  - Client encrypts JSON payload before sending
  - Server's `DecryptionMiddleware` decrypts incoming requests
  - Decrypted data flows to controllers

### 2. **Response Payload Encryption** (Server → Client)
- **Technology**: AES-CBC with shared secret key
- **Flow**:
  - Controllers return plain JSON
  - Server's `EncryptionMiddleware` encrypts responses
  - Client decrypts response data automatically

### 3. **JWT Claim Encryption** (Server-side)
- **Technology**: OS-based encryption (machine-specific)
- **Flow**:
  - User data (uid, email) encrypted into "data" claim
  - JWT structure: `{ "data": "encrypted_user_data", "exp": ..., "nbf": ..., "iat": ... }`
  - Standard claims (exp, nbf, iat) remain unencrypted for JWT validation
  - Decrypted during JWT validation middleware

### 4. **Password Security**
- **Technology**: SHA256 hashing
- **Flow**: Passwords hashed before storage, never stored in plain text

### 5. **Transport Security**
- **Technology**: HTTPS/TLS
- **Flow**: All communication over encrypted HTTPS channel

## Shared Secret Key

Both client and server use the same secret key for payload encryption:

```
LibraryManagement_SecretKey_2024_DoNotShare
```

**Key Derivation**:
- SHA256 hash of secret key
- First 32 bytes = AES key
- First 16 bytes = IV (Initialization Vector)

## Testing the Encryption

### Step 1: Start Backend
```bash
cd src/LibraryManagement.API
dotnet run
```

Backend runs on: `https://localhost:7146`

### Step 2: Start Frontend
```bash
cd frontend
npx http-server -p 8000
```

Frontend runs on: `http://localhost:8000` (must use localhost for Web Crypto API)

### Step 3: Test Encryption Flow

1. **Open Browser DevTools** (F12)
2. **Go to Network Tab**
3. **Access**: `http://localhost:8000`
4. **Signup/Login**: Create account or login
5. **Observe Network Requests**:
   - Request payload: Encrypted (text/plain)
   - Response payload: Encrypted (text/plain)
   - JWT token: Contains encrypted "data" claim

### Step 4: Verify Encryption

**Check Request Encryption**:
```
POST /api/auth/signup
Content-Type: text/plain

[Base64 encrypted string]
```

**Check Response Encryption**:
```
Response Content-Type: text/plain

[Base64 encrypted string]
```

**Check JWT Token** (paste in jwt.io):
```json
{
  "data": "8ppZywUv8ale2xdnQZITMA==...",
  "exp": 1768554717,
  "nbf": 1768468317,
  "iat": 1768468317
}
```

## Encryption Test in Console

Open browser console on frontend and run:
```javascript
// Test encryption
testEncryption();
```

Expected output:
```
Original: Hello, Library!
Encrypted: [Base64 string]
Decrypted: Hello, Library!
Encryption test: ✅ PASSED
```

## API Endpoints (All Encrypted)

### Authentication
- `POST /api/auth/signup` - Encrypted request/response
- `POST /api/auth/login` - Encrypted request/response

### Books (Requires JWT)
- `GET /api/books` - Encrypted response
- `GET /api/books/{id}` - Encrypted response
- `POST /api/books` - Encrypted request/response
- `DELETE /api/books/{id}` - Encrypted response

## Security Features

✅ **Request encryption** - All POST requests encrypted  
✅ **Response encryption** - All successful responses encrypted  
✅ **JWT claim encryption** - User data in JWT encrypted  
✅ **Password hashing** - SHA256 hashing  
✅ **HTTPS transport** - TLS encryption  
✅ **User isolation** - Each user sees only their books  
✅ **Token validation** - JWT expiry and signature validation  
✅ **Comprehensive logging** - All operations logged  

## Middleware Pipeline

```
Request Flow:
Client → HTTPS → CORS → ExceptionHandling → DecryptionMiddleware → 
Authentication → Authorization → JwtValidation → Controller → 
EncryptionMiddleware → HTTPS → Client
```

## Troubleshooting

### "crypto.subtle is undefined"
- **Cause**: Web Crypto API requires HTTPS or localhost
- **Solution**: Access via `http://localhost:8000` (not IP address)

### "Encryption failed"
- **Cause**: Encryption function not loaded
- **Solution**: Ensure `encryption.js` is loaded before `app.js`

### "401 Unauthorized"
- **Cause**: Invalid or expired JWT token
- **Solution**: Login again to get new token

### "415 Unsupported Media Type"
- **Cause**: Content-Type not set correctly after decryption
- **Solution**: Already fixed in DecryptionMiddleware

## Files Involved

### Backend
- `DecryptionMiddleware.cs` - Decrypts incoming requests
- `EncryptionMiddleware.cs` - Encrypts outgoing responses
- `PayloadEncryptionService.cs` - Shared secret encryption
- `EncryptionService.cs` - OS-based JWT encryption
- `JwtService.cs` - JWT generation with encrypted claims

### Frontend
- `encryption.js` - Client-side AES-CBC encryption
- `app.js` - API calls with encryption/decryption

## Next Steps

1. ✅ Request encryption implemented
2. ✅ Response encryption implemented
3. ✅ JWT claim encryption implemented
4. ✅ Comprehensive logging added
5. ⏳ Session management (token expiry handling)
6. ⏳ Route protection (prevent URL navigation without login)
7. ⏳ Separate page for user's books

## Notes

- The system is now **fully end-to-end encrypted**
- All data transmission is encrypted (requests, responses, JWT claims)
- Only standard JWT claims (exp, nbf, iat) remain unencrypted for validation
- User data in JWT is encrypted in the "data" claim
- Frontend automatically handles encryption/decryption
