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
            
            // Encrypt individual claim values
            var encryptedUserId = _encryptionService.Encrypt(userId.ToString());
            var encryptedEmail = _encryptionService.Encrypt(email);
            var encryptedJti = _encryptionService.Encrypt(Guid.NewGuid().ToString());
            var encryptedIssuer = _encryptionService.Encrypt(_issuer);
            var encryptedAudience = _encryptionService.Encrypt(_audience);
            
            var now = DateTime.UtcNow;
            var expires = now.AddHours(24);
            
            // Encrypt timestamp values as strings
            var encryptedNbf = _encryptionService.Encrypt(new DateTimeOffset(now).ToUnixTimeSeconds().ToString());
            var encryptedExp = _encryptionService.Encrypt(new DateTimeOffset(expires).ToUnixTimeSeconds().ToString());
            var encryptedIat = _encryptionService.Encrypt(new DateTimeOffset(now).ToUnixTimeSeconds().ToString());
            
            // Create JWT with encrypted claim values
            // We add encrypted iss and aud as custom claims since standard ones need to be plain for validation
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("uid", encryptedUserId),
                    new Claim("eml", encryptedEmail),
                    new Claim("jti", encryptedJti),
                    new Claim("nbf", encryptedNbf),
                    new Claim("exp", encryptedExp),
                    new Claim("iat", encryptedIat),
                    new Claim("iss", encryptedIssuer),      // Encrypted issuer in standard claim
                    new Claim("aud", encryptedAudience)     // Encrypted audience in standard claim
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
                // Disable issuer/audience validation since they're encrypted
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
                // Note: exp, nbf, iat might be numeric (added by JWT library) or encrypted strings (our custom claims)
                var encryptedUserId = principal.FindFirst("uid")?.Value;
                var encryptedIssuer = principal.FindFirst("iss")?.Value;
                var encryptedAudience = principal.FindFirst("aud")?.Value;
                
                if (string.IsNullOrEmpty(encryptedUserId))
                    return null;
                
                // Decrypt user ID
                var userIdStr = _encryptionService.Decrypt(encryptedUserId);
                var userId = int.Parse(userIdStr);
                
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
                
                // Note: exp, nbf, iat are validated by JWT library (they're numeric timestamps)
                // We don't need to decrypt them since they're automatically added by the library
                
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
