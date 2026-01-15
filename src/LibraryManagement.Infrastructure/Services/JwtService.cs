using LibraryManagement.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
            
            // Encrypt sensitive data before putting in JWT
            var encryptedUserId = _encryptionService.Encrypt(userId.ToString());
            var encryptedEmail = _encryptionService.Encrypt(email);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("uid", encryptedUserId), // Encrypted user ID
                    new Claim("eml", encryptedEmail),  // Encrypted email
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
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
                
                // Get encrypted user ID and decrypt it
                var encryptedUserId = jwtToken.Claims.FirstOrDefault(x => x.Type == "uid")?.Value;
                
                if (string.IsNullOrEmpty(encryptedUserId))
                    return null;
                
                var decryptedUserId = _encryptionService.Decrypt(encryptedUserId);
                return int.Parse(decryptedUserId);
            }
            catch
            {
                return null;
            }
        }
    }
}
