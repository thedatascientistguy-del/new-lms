using LibraryManagement.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace LibraryManagement.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IEncryptionService _encryptionService;

        public JwtService(string secretKey, string issuer, string audience, IEncryptionService encryptionService)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
            _encryptionService = encryptionService;
        }

        public string GenerateToken(int userId, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            
            // Create the complete payload with all claims
            var payload = new
            {
                uid = userId,
                eml = email,
                jti = Guid.NewGuid().ToString(),
                nbf = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                exp = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds(),
                iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                iss = _issuer,
                aud = _audience
            };
            
            // Serialize the entire payload to JSON
            var payloadJson = JsonSerializer.Serialize(payload);
            
            // Encrypt the entire payload
            var encryptedPayload = _encryptionService.Encrypt(payloadJson);
            
            // Return ONLY the encrypted string (no JWT wrapper)
            // This way, EVERYTHING is encrypted - no visible claims at all
            return encryptedPayload;
        }

        public int? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                // Decrypt the token (it's just an encrypted string now)
                var decryptedJson = _encryptionService.Decrypt(token);
                
                // Deserialize to get the payload
                var payloadDoc = JsonDocument.Parse(decryptedJson);
                var userId = payloadDoc.RootElement.GetProperty("uid").GetInt32();
                
                // Validate expiration from decrypted payload
                var exp = payloadDoc.RootElement.GetProperty("exp").GetInt64();
                var expDateTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                
                if (expDateTime < DateTime.UtcNow)
                    return null; // Token expired
                
                // Validate issuer
                var iss = payloadDoc.RootElement.GetProperty("iss").GetString();
                if (iss != _issuer)
                    return null;
                
                // Validate audience
                var aud = payloadDoc.RootElement.GetProperty("aud").GetString();
                if (aud != _audience)
                    return null;
                
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
