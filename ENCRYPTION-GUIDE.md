# Encryption Guide

## 🔐 How Encryption Works in This API

The API includes an **optional encryption middleware** that can encrypt/decrypt payloads for additional security.

## ✅ Current Configuration: Smart Detection

The middleware now **automatically detects** whether your payload is:
- **Plain JSON** (starts with `{` or `[`) → Passes through without decryption
- **Encrypted** (Base64 string) → Attempts to decrypt

This means you can use **both plain JSON and encrypted payloads** without any configuration changes!

## 🚀 For Testing: Use Plain JSON (Recommended)

Just send regular JSON payloads as shown in the testing guide:

```json
{
  "username": "testuser",
  "email": "test@library.com",
  "password": "Test@123456"
}
```

The middleware will detect it's JSON and pass it through. **No encryption needed for testing!**

## 🔒 For Production: Use Encrypted Payloads (Optional)

If you want to use encryption in production:

### Step 1: Encrypt Your Payload

The encryption uses **OS-based AES encryption** with machine-specific keys.

**Example using C#:**
```csharp
var encryptionService = new EncryptionService();
var jsonPayload = "{\"username\":\"testuser\",\"email\":\"test@library.com\",\"password\":\"Test@123456\"}";
var encrypted = encryptionService.Encrypt(jsonPayload);
// Send 'encrypted' as the request body
```

### Step 2: Send Encrypted String

Instead of sending JSON, send the encrypted Base64 string:

```
POST /api/auth/signup
Content-Type: application/json

"U2FsdGVkX1+vupppZksvRf5pq5g5XjFRlipRkwB0K1Y96Qsv2Lm+31cmzaAILwyt..."
```

### Step 3: Middleware Decrypts Automatically

The middleware will:
1. Detect it's not JSON (doesn't start with `{`)
2. Decrypt the payload using OS-based keys
3. Pass the decrypted JSON to your controller

## 🛠️ Configuration Options

### Option 1: Keep Smart Detection (Current - Recommended)

**Status:** ✅ Enabled  
**File:** `src/LibraryManagement.API/Program.cs`

```csharp
app.UseMiddleware<DecryptionMiddleware>(); // Supports both JSON and encrypted
```

**Pros:**
- Works with plain JSON for testing
- Works with encrypted payloads for production
- No configuration needed

**Cons:**
- Slightly less secure (allows unencrypted payloads)

### Option 2: Force Encryption (Production)

To **require** encryption for all POST/PUT requests:

**File:** `src/LibraryManagement.API/Middleware/DecryptionMiddleware.cs`

Change this line:
```csharp
bool isJson = trimmedBody.StartsWith("{") || trimmedBody.StartsWith("[");

if (isJson)
{
    // Body is already JSON, no decryption needed
    context.Request.Body.Position = 0;
}
```

To:
```csharp
bool isJson = trimmedBody.StartsWith("{") || trimmedBody.StartsWith("[");

if (isJson)
{
    // Reject unencrypted payloads in production
    context.Response.StatusCode = 400;
    await context.Response.WriteAsync("Encrypted payload required");
    return;
}
```

### Option 3: Disable Encryption Completely

To disable encryption middleware entirely:

**File:** `src/LibraryManagement.API/Program.cs`

Comment out the middleware:
```csharp
// app.UseMiddleware<DecryptionMiddleware>(); // Encryption disabled
```

## 🔑 How OS-Based Encryption Works

The encryption service uses:
- **Machine Name** + **OS Version** as entropy
- **SHA256** to derive encryption keys
- **AES** for symmetric encryption
- **Base64** encoding for transport

This means:
- Encrypted payloads are **machine-specific**
- Same payload encrypted on different machines produces different results
- Provides an additional layer of security

## 📝 Testing Encrypted Payloads

If you want to test with encryption:

### Using PowerShell

```powershell
# This is a simplified example - actual encryption is more complex
$plainText = '{"username":"testuser","email":"test@library.com","password":"Test@123456"}'
$bytes = [System.Text.Encoding]::UTF8.GetBytes($plainText)
$base64 = [Convert]::ToBase64String($bytes)
Write-Host $base64
```

### Using C# Console App

```csharp
using LibraryManagement.Infrastructure.Services;

var encryptionService = new EncryptionService();
var json = "{\"username\":\"testuser\",\"email\":\"test@library.com\",\"password\":\"Test@123456\"}";
var encrypted = encryptionService.Encrypt(json);
Console.WriteLine(encrypted);
```

Then send the encrypted string as the request body.

## ⚠️ Important Notes

1. **For Testing:** Use plain JSON - it's easier and the middleware supports it
2. **For Production:** Consider requiring encryption for sensitive endpoints
3. **Machine-Specific:** Encrypted payloads only work on the same machine (due to OS-based keys)
4. **Auth Endpoints:** Signup and Login should probably allow plain JSON for client compatibility
5. **Book Endpoints:** Can require encryption if needed

## 🎯 Recommended Setup

### Development/Testing
```csharp
// Allow both plain JSON and encrypted
app.UseMiddleware<DecryptionMiddleware>();
```

### Production
```csharp
// Option 1: Keep flexible (recommended for APIs)
app.UseMiddleware<DecryptionMiddleware>();

// Option 2: Require encryption (modify middleware to reject plain JSON)
// Modify DecryptionMiddleware.cs as shown in Option 2 above
```

## 🔄 Current Status

✅ **Encryption middleware is ENABLED**  
✅ **Supports plain JSON** (for easy testing)  
✅ **Supports encrypted payloads** (for production)  
✅ **Automatic detection** (no configuration needed)

## 🚀 Quick Start

**Just use plain JSON for testing!** The middleware will handle it automatically.

```json
POST /api/auth/signup
{
  "username": "testuser",
  "email": "test@library.com",
  "password": "Test@123456"
}
```

No encryption needed! 🎉

## 📞 Troubleshooting

### Error: "Invalid encrypted payload"

**Cause:** You sent something that's not JSON and not valid encrypted data

**Solution:** 
- Make sure your payload is valid JSON starting with `{`
- OR send properly encrypted Base64 string

### Error: "Encrypted payload required"

**Cause:** Middleware is configured to require encryption

**Solution:**
- Either encrypt your payload
- OR change middleware to allow plain JSON (see Option 1 above)

## 💡 Summary

- ✅ **Current setup:** Works with plain JSON (easy testing)
- ✅ **Also supports:** Encrypted payloads (production security)
- ✅ **No configuration needed:** Automatic detection
- ✅ **Just send JSON:** The middleware handles the rest

Happy testing! 🎉
