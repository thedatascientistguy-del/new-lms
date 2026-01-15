# Encryption Architecture

## Overview
The Library Management System uses a **hybrid encryption approach** with two separate encryption systems for different purposes.

---

## 🔐 Two Encryption Systems

### 1. **OS-Based Encryption** (Server-Side Only)
**Used For:** JWT Token Claims

**Implementation:** `EncryptionService.cs`

**How It Works:**
- Uses machine-specific entropy (Machine Name + OS Version)
- Generates unique encryption key per server
- AES-256 encryption with SHA-256 key derivation
- Key: `SHA256(MachineName_OSVersion_LibraryManagement)`

**What It Encrypts:**
- JWT claim values (uid, eml, jti, iss, aud)
- Stored in "data" claim as encrypted JSON

**Why OS-Based:**
- ✅ Maximum security for server-side data
- ✅ Key never leaves the server
- ✅ Unique per deployment environment
- ✅ Cannot be compromised by client

**Example:**
```csharp
// Server generates JWT with encrypted claims
var userData = new { uid = 1, eml = "user@example.com" };
var encrypted = _encryptionService.Encrypt(JsonSerializer.Serialize(userData));
// Token contains: { "data": "encrypted_string", "exp": 1234567890 }
```

---

### 2. **Shared Secret Encryption** (Client-Server)
**Used For:** Request/Response Payloads

**Implementation:** `PayloadEncryptionService.cs`

**How It Works:**
- Uses shared secret key known by both client and server
- AES-256 encryption with SHA-256 key derivation
- Key: `SHA256("LibraryManagement_SecretKey_2024_DoNotShare")`
- Same key on client (JavaScript) and server (C#)

**What It Encrypts:**
- Login/Signup request payloads
- Add Book request payloads
- Any POST/PUT request body (optional)

**Why Shared Secret:**
- ✅ Client can encrypt data before sending
- ✅ Server can decrypt incoming requests
- ✅ Protects sensitive data in transit (beyond HTTPS)
- ✅ Works across different machines

**Example:**
```javascript
// Client encrypts payload
const payload = { email: "user@example.com", password: "secret" };
const encrypted = await encryptString(JSON.stringify(payload));
// Sends encrypted string to server
```

```csharp
// Server decrypts payload
var decrypted = _payloadEncryptionService.Decrypt(encryptedBody);
var loginRequest = JsonSerializer.Deserialize<LoginRequest>(decrypted);
```

---

## 🔄 Request Flow

### Encrypted Request (e.g., Login)

```
Client                          Server
  │                               │
  │  1. Create JSON payload       │
  │     { email, password }       │
  │                               │
  │  2. Encrypt with shared key   │
  │     "kL3EtL/liiJsggpkRl..."   │
  │                               │
  │  3. Send encrypted string ────┼──> DecryptionMiddleware
  │                               │    │
  │                               │    │ 4. Decrypt with shared key
  │                               │    │
  │                               │    ▼
  │                               │   AuthController
  │                               │    │
  │                               │    │ 5. Process login
  │                               │    │ 6. Generate JWT
  │                               │    │    - Encrypt claims with OS-based
  │                               │    │
  │  <────────────────────────────┼────┘
  │  7. Receive JWT token         │
  │     (claims encrypted)        │
```

### Authenticated Request (e.g., Get Books)

```
Client                          Server
  │                               │
  │  1. Add JWT to header         │
  │     Authorization: Bearer ... │
  │                               │
  │  2. Send request ─────────────┼──> JwtValidationMiddleware
  │                               │    │
  │                               │    │ 3. Validate JWT signature
  │                               │    │ 4. Decrypt "data" claim (OS-based)
  │                               │    │ 5. Extract user ID
  │                               │    │
  │                               │    ▼
  │                               │   BooksController
  │                               │    │
  │                               │    │ 6. Get books from DB
  │                               │    │
  │  <────────────────────────────┼────┘
  │  7. Receive books (plain JSON)│
```

---

## 🛡️ Security Layers

### Layer 1: HTTPS (Transport)
- All communication encrypted in transit
- TLS 1.2/1.3
- Prevents man-in-the-middle attacks

### Layer 2: Payload Encryption (Application)
- Request bodies encrypted with shared secret
- Protects data even if HTTPS is compromised
- Optional but recommended for sensitive operations

### Layer 3: JWT Encryption (Token)
- JWT claims encrypted with OS-based key
- Token data unreadable even if intercepted
- Server-side only, maximum security

### Layer 4: Password Hashing (Storage)
- Passwords hashed with SHA256
- Never stored in plain text
- One-way encryption

---

## 📁 File Structure

```
Backend:
├── EncryptionService.cs           ← OS-based (JWT)
├── PayloadEncryptionService.cs    ← Shared secret (Payloads)
├── DecryptionMiddleware.cs        ← Uses PayloadEncryptionService
├── JwtService.cs                  ← Uses EncryptionService
└── JwtValidationMiddleware.cs     ← Uses JwtService

Frontend:
├── encryption.js                  ← Shared secret encryption
└── app.js                         ← Uses encryption.js
```

---

## 🔑 Encryption Keys

### OS-Based Key (Server Only)
```
Input: MachineName + OSVersion + "LibraryManagement"
Process: SHA256 hash
Output: 32-byte key + 16-byte IV
Example: "SERVER-PC_Windows 10_LibraryManagement" → SHA256 → Key
```

### Shared Secret Key (Client + Server)
```
Input: "LibraryManagement_SecretKey_2024_DoNotShare"
Process: SHA256 hash
Output: 32-byte key + 16-byte IV
Same on both client and server
```

---

## 🧪 Testing Encryption

### Test Backend Encryption
```bash
dotnet run --project src/LibraryManagement.API
# Check Swagger: https://localhost:7001/swagger
# Test signup/login - JWT should have encrypted "data" claim
```

### Test Frontend Encryption
```bash
cd frontend
python -m http.server 8000
# Open browser console (F12)
# Check "Encryption test: ✅ PASSED" message
# Test signup - payload should be encrypted in Network tab
```

### Verify Encryption
1. **JWT Claims**: Decode token on jwt.io - "data" field is encrypted
2. **Request Payload**: Check Network tab - POST body is encrypted string
3. **Decryption**: Server logs show decrypted data (if logging enabled)

---

## 🔄 Encryption Toggle

### Enable/Disable Payload Encryption

**Frontend (app.js):**
```javascript
// Encrypt payload
await apiCall('/api/auth/login', 'POST', { email, password }, true);

// Plain JSON (no encryption)
await apiCall('/api/auth/login', 'POST', { email, password }, false);
```

**Backend (DecryptionMiddleware):**
- Automatically detects encrypted vs plain JSON
- If starts with `{` or `[` → Plain JSON
- Otherwise → Encrypted, decrypt it

---

## 🚀 Production Recommendations

1. **Change Shared Secret Key**
   - Update in `PayloadEncryptionService.cs`
   - Update in `frontend/encryption.js`
   - Use environment variable in production

2. **Use HTTPS Only**
   - Never use HTTP in production
   - Enforce HTTPS redirects

3. **Rotate Keys Periodically**
   - Change shared secret every 6-12 months
   - Update both client and server

4. **Monitor Decryption Failures**
   - Log failed decryption attempts
   - Alert on suspicious patterns

5. **Consider Key Management Service**
   - Azure Key Vault
   - AWS KMS
   - HashiCorp Vault

---

## ❓ FAQ

**Q: Why two encryption systems?**
A: OS-based is more secure but only works server-side. Shared secret allows client encryption.

**Q: Can I use only one encryption system?**
A: Yes, but you lose either client encryption (OS-based only) or maximum JWT security (shared only).

**Q: Is the shared secret secure?**
A: Yes, when combined with HTTPS. The key is never transmitted, only used locally.

**Q: What if someone gets the shared secret?**
A: They can decrypt payloads but not JWT claims (OS-based). Change the key immediately.

**Q: Can I disable payload encryption?**
A: Yes, set `encrypt: false` in frontend API calls. Middleware accepts plain JSON.

**Q: Is this overkill for a library system?**
A: For learning purposes, yes. For production with sensitive data, it's good practice.

---

## 📊 Encryption Comparison

| Feature | OS-Based | Shared Secret |
|---------|----------|---------------|
| **Used For** | JWT Claims | Request Payloads |
| **Client Can Encrypt** | ❌ No | ✅ Yes |
| **Server Can Decrypt** | ✅ Yes | ✅ Yes |
| **Key Location** | Server Only | Client + Server |
| **Security Level** | Maximum | High |
| **Cross-Machine** | ❌ No | ✅ Yes |
| **Algorithm** | AES-256-CBC | AES-256-CBC |
| **Key Derivation** | SHA-256 | SHA-256 |

---

## 🎯 Summary

✅ **JWT Claims**: Encrypted with OS-based key (server-side only)
✅ **Request Payloads**: Encrypted with shared secret (client-server)
✅ **HTTPS**: All traffic encrypted in transit
✅ **Passwords**: Hashed with SHA256
✅ **Flexible**: Can use plain JSON or encrypted payloads
✅ **Secure**: Multiple layers of encryption

Your Library Management System now has enterprise-grade encryption! 🔐
