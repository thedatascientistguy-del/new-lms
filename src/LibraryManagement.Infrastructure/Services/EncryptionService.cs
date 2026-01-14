using LibraryManagement.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace LibraryManagement.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService()
        {
            // OS-based encryption using Data Protection API (DPAPI) on Windows
            // For cross-platform, we use a derived key from machine-specific entropy
            var entropy = GetMachineEntropy();
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(entropy);
            _key = hash;
            _iv = hash.Take(16).ToArray();
        }

        private byte[] GetMachineEntropy()
        {
            var machineName = Environment.MachineName;
            var osVersion = Environment.OSVersion.ToString();
            var entropy = $"{machineName}_{osVersion}_LibraryManagement";
            return Encoding.UTF8.GetBytes(entropy);
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
