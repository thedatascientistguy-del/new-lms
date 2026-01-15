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
            
            var now = DateTime.UtcNow;
            var expires = now.AddHours(24);
            
            // Create user data object with uid and email
            var userData = new
            {
                uid = userId,
                eml = email
            };
            
            // Serialize and encrypt user data into single "data" field
            var userDataJson = JsonSerializer.Serialize(userData);
            var encryptedData = _encryptionService.Encrypt(userDataJson);
            
            // Encrypt other claim values individually
            var encryptedJti = _encryptionService.Encrypt(Guid.NewGuid().ToString());
            var encryptedIssuer = _encryptionService.Encrypt(_issuer);
            var encryptedAudience = _encryptionService.Encrypt(_audience);
            var encryptedNbf = _encryptionService.Encrypt(new DateTimeOffset(now).ToUnixTimeSeconds().ToString());
            var encryptedExp = _encryptionService.Encrypt(new DateTimeOffset(expires).ToUnixTimeSeconds().ToString());
            var encryptedIat = _encryptionService.Encrypt(new DateTimeOffset(now).ToUnixTimeSeconds().ToString());
            
            // Create JWT with encrypted claims
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("data", encryptedData),      // User data (uid, eml) encrypted together
                    new Claim("jti", encryptedJti),
                    new Claim("nbf", encryptedNbf),
                    new Claim("exp", encryptedExp),
                    new Claim("iat", encryptedIat),
                    new Claim("iss", encryptedIssuer),
                    new Claim("aud", encryptedAudience)
                }),
                Expires = expires,  // Standard JWT expiration for validation
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

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);
                
                // Validate JWT signature and structure
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,  // We'll validate manually after decryption
                    ValidateAudience = false, // We'll validate manually after decryption
                    ValidateLifetime = true, // Validates standard exp claim
                    ClockSkew = TimeSpan.Zero
                };
                
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                
                // Extract encrypted claims
                var encryptedData = principal.FindFirst("data")?.Value;
                var encryptedIssuer = principal.FindFirst("iss")?.Value;
                var encryptedAudience = principal.FindFirst("aud")?.Value;
                
                if (string.IsNullOrEmpty(encryptedData))
                    return null;
                
                // Decrypt and parse user data
                var decryptedJson = _encryptionService.Decrypt(encryptedData);
                var userData = JsonDocument.Parse(decryptedJson);
                
                // Extract user ID from decrypted data
                var userId = userData.RootElement.GetProperty("uid").GetInt32();
                
                // Decrypt and validate issuer
                if (!string.IsNullOrEmpty(encryptedIssuer))
                {
                    var issuer = _encryptionService.Decrypt(encryptedIssuer);
                    if (issuer != _issuer)
                        return null;
                }
                
                // Decrypt and validate audience
                if (!string.IsNullOrEmpty(encryptedAudience))
                {
                    var audience = _encryptionService.Decrypt(encryptedAudience);
                    if (audience != _audience)
                        return null;
                }
                
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
