using LibraryManagement.Core.Interfaces;
using System.Text;

namespace LibraryManagement.API.Middleware
{
    public class DecryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEncryptionService _encryptionService;

        public DecryptionMiddleware(RequestDelegate next, IEncryptionService encryptionService)
        {
            _next = next;
            _encryptionService = encryptionService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering();
                
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                
                if (!string.IsNullOrEmpty(body))
                {
                    // Check if the body is already valid JSON (starts with { or [)
                    var trimmedBody = body.TrimStart();
                    bool isJson = trimmedBody.StartsWith("{") || trimmedBody.StartsWith("[");
                    
                    if (isJson)
                    {
                        // Body is already JSON, no decryption needed
                        context.Request.Body.Position = 0;
                    }
                    else
                    {
                        // Body appears to be encrypted, try to decrypt
                        try
                        {
                            var decryptedBody = _encryptionService.Decrypt(body);
                            var bytes = Encoding.UTF8.GetBytes(decryptedBody);
                            context.Request.Body = new MemoryStream(bytes);
                            context.Request.Body.Position = 0;
                        }
                        catch
                        {
                            context.Response.StatusCode = 400;
                            await context.Response.WriteAsync("Invalid encrypted payload");
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
