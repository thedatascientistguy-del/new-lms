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
            
            // Create JWT with only the encrypted payload as a single claim
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("data", encryptedPayload) // All data encrypted in one claim
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                var jwtToken = (JwtSecurityToken)validatedToken;
                
                // Get the encrypted data claim
                var encryptedData = jwtToken.Claims.FirstOrDefault(x => x.Type == "data")?.Value;
                
                if (string.IsNullOrEmpty(encryptedData))
                    return null;
                
                // Decrypt the entire payload
                var decryptedJson = _encryptionService.Decrypt(encryptedData);
                
                // Deserialize to get the user ID
                var payloadDoc = JsonDocument.Parse(decryptedJson);
                var userId = payloadDoc.RootElement.GetProperty("uid").GetInt32();
                
                // Validate expiration from decrypted payload
                var exp = payloadDoc.RootElement.GetProperty("exp").GetInt64();
                var expDateTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                
                if (expDateTime < DateTime.UtcNow)
                    return null; // Token expired
                
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
