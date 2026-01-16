using LibraryManagement.Core.Interfaces;
using System.Text;

namespace LibraryManagement.API.Middleware
{
    public class DecryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPayloadEncryptionService _payloadEncryptionService;
        private readonly ILogger<DecryptionMiddleware> _logger;

        public DecryptionMiddleware(RequestDelegate next, IPayloadEncryptionService payloadEncryptionService, ILogger<DecryptionMiddleware> logger)
        {
            _next = next;
            _payloadEncryptionService = payloadEncryptionService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering();
                
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                
                _logger.LogInformation("Request body received: {BodyLength} characters, ContentType: {ContentType}", 
                    body?.Length ?? 0, context.Request.ContentType);
                
                if (!string.IsNullOrEmpty(body))
                {
                    // Check if the body is already valid JSON (starts with { or [)
                    var trimmedBody = body.TrimStart();
                    bool isJson = trimmedBody.StartsWith("{") || trimmedBody.StartsWith("[");
                    
                    if (isJson)
                    {
                        _logger.LogInformation("Body is already JSON, no decryption needed");
                        // Body is already JSON, no decryption needed
                        context.Request.Body.Position = 0;
                    }
                    else
                    {
                        _logger.LogInformation("Body appears encrypted, attempting decryption. Preview: {Preview}", 
                            body.Length > 50 ? body.Substring(0, 50) + "..." : body);
                        
                        // Body appears to be encrypted, try to decrypt
                        try
                        {
                            var decryptedBody = _payloadEncryptionService.Decrypt(body);
                            _logger.LogInformation("Decryption successful. Decrypted body: {DecryptedBody}", decryptedBody);
                            
                            var bytes = Encoding.UTF8.GetBytes(decryptedBody);
                            context.Request.Body = new MemoryStream(bytes);
                            context.Request.Body.Position = 0;
                            
                            // Change Content-Type back to JSON after decryption
                            context.Request.ContentType = "application/json";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Decryption failed for body: {Body}", body);
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync($"Invalid encrypted payload: {ex.Message}");
                            return;
                        }
                    }
                }
                else
                {
                    context.Request.Body.Position = 0;
                }
            }

            await _next(context);
        }
    }
}
