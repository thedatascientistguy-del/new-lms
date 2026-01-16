// Encryption utilities for client-side encryption
// Matches the backend AES encryption implementation

// Shared secret key (must match backend)
const SECRET_KEY = "LibraryManagement_SecretKey_2024_DoNotShare";

// Convert string to ArrayBuffer
function str2ab(str) {
    const buf = new ArrayBuffer(str.length);
    const bufView = new Uint8Array(buf);
    for (let i = 0; i < str.length; i++) {
        bufView[i] = str.charCodeAt(i);
    }
    return buf;
}

// Convert ArrayBuffer to Base64
function arrayBufferToBase64(buffer) {
    let binary = '';
    const bytes = new Uint8Array(buffer);
    const len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
}

// Convert Base64 to ArrayBuffer
function base64ToArrayBuffer(base64) {
    const binary_string = atob(base64);
    const len = binary_string.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes.buffer;
}

// Generate key and IV from secret (matches backend SHA256 logic)
async function deriveKey() {
    const encoder = new TextEncoder();
    const keyMaterial = encoder.encode(SECRET_KEY);
    
    // Check if crypto.subtle is available (requires HTTPS or localhost)
    if (!window.crypto || !window.crypto.subtle) {
        throw new Error('Web Crypto API not available. Please use HTTPS or localhost.');
    }
    
    // Hash with SHA-256 (matches backend)
    const hashBuffer = await window.crypto.subtle.digest('SHA-256', keyMaterial);
    const hashArray = new Uint8Array(hashBuffer);
    
    // Key is full 32 bytes, IV is first 16 bytes (matches backend)
    const key = hashArray;
    const iv = hashArray.slice(0, 16);
    
    return { key, iv };
}

// Import key for Web Crypto API
async function importKey(keyBytes) {
    return await window.crypto.subtle.importKey(
        'raw',
        keyBytes,
        { name: 'AES-CBC' },
        false,
        ['encrypt', 'decrypt']
    );
}

// Encrypt string (matches backend AES-CBC encryption)
async function encryptString(plainText) {
    if (!plainText) return plainText;
    
    try {
        const { key, iv } = await deriveKey();
        const cryptoKey = await importKey(key);
        
        const encoder = new TextEncoder();
        const data = encoder.encode(plainText);
        
        const encrypted = await window.crypto.subtle.encrypt(
            { name: 'AES-CBC', iv: iv },
            cryptoKey,
            data
        );
        
        return arrayBufferToBase64(encrypted);
    } catch (error) {
        console.error('Encryption error:', error);
        throw error;
    }
}

// Decrypt string (matches backend AES-CBC decryption)
async function decryptString(cipherText) {
    if (!cipherText) return cipherText;
    
    try {
        const { key, iv } = await deriveKey();
        const cryptoKey = await importKey(key);
        
        const encryptedData = base64ToArrayBuffer(cipherText);
        
        const decrypted = await window.crypto.subtle.decrypt(
            { name: 'AES-CBC', iv: iv },
            cryptoKey,
            encryptedData
        );
        
        const decoder = new TextDecoder();
        return decoder.decode(decrypted);
    } catch (error) {
        console.error('Decryption error:', error);
        throw error;
    }
}

// Test encryption/decryption
async function testEncryption() {
    const testData = "Hello, Library!";
    console.log('Original:', testData);
    
    const encrypted = await encryptString(testData);
    console.log('Encrypted:', encrypted);
    
    const decrypted = await decryptString(encrypted);
    console.log('Decrypted:', decrypted);
    
    console.log('Encryption test:', testData === decrypted ? '✅ PASSED' : '❌ FAILED');
}

// Run test on load
testEncryption().catch(console.error);

console.log('Encryption module loaded with AES-CBC encryption');
