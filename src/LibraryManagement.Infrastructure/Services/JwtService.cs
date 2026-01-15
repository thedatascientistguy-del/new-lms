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
            
            // Create JWT with encrypted claim VALUES (structure is visible, data is not)
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
                    new Claim("iss", encryptedIssuer),
                    new Claim("aud", encryptedAudience)
                }),
                Expires = expires,
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
                    ValidateIssuer = false, // We'll validate manually after decryption
                    ValidateAudience = false, // We'll validate manually after decryption
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                
                // Extract and decrypt claims
                var encryptedUserId = principal.FindFirst("uid")?.Value;
                var encryptedExp = principal.FindFirst("exp")?.Value;
                var encryptedIssuer = principal.FindFirst("iss")?.Value;
                var encryptedAudience = principal.FindFirst("aud")?.Value;
                
                if (string.IsNullOrEmpty(encryptedUserId))
                    return null;
                
                // Decrypt claim values
                var userIdStr = _encryptionService.Decrypt(encryptedUserId);
                var userId = int.Parse(userIdStr);
                
                // Validate expiration
                if (!string.IsNullOrEmpty(encryptedExp))
                {
                    var expStr = _encryptionService.Decrypt(encryptedExp);
                    var exp = long.Parse(expStr);
                    var expDateTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    
                    if (expDateTime < DateTime.UtcNow)
                        return null;
                }
                
                // Validate issuer
                if (!string.IsNullOrEmpty(encryptedIssuer))
                {
                    var issuer = _encryptionService.Decrypt(encryptedIssuer);
                    if (issuer != _issuer)
                        return null;
                }
                
                // Validate audience
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
