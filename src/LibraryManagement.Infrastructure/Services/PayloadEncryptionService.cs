using LibraryManagement.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagement.Infrastructure.Services
{
    public class PayloadEncryptionService : IPayloadEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public PayloadEncryptionService()
        {
            // Shared secret key for client-server payload encryption
            // This key is shared between client and server
            var secretKey = "LibraryManagement_SecretKey_2024_DoNotShare";
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
            _key = hash;
            _iv = hash.Take(16).ToArray();
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
