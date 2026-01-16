using LibraryManagement.Core.Interfaces;
using System.Text;

namespace LibraryManagement.API.Middleware
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPayloadEncryptionService _payloadEncryptionService;
        private readonly ILogger<EncryptionMiddleware> _logger;

        public EncryptionMiddleware(RequestDelegate next, IPayloadEncryptionService payloadEncryptionService, ILogger<EncryptionMiddleware> logger)
        {
            _next = next;
            _payloadEncryptionService = payloadEncryptionService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Store original response body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                // Create a new memory stream to capture the response
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Call the next middleware
                await _next(context);

                _logger.LogInformation("Response status: {StatusCode}, ContentType: {ContentType}", 
                    context.Response.StatusCode, context.Response.ContentType);

                // Only encrypt successful responses (200-299)
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    // Read the response
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    _logger.LogInformation("Response body length: {Length}, Preview: {Preview}", 
                        responseText?.Length ?? 0, 
                        responseText?.Length > 100 ? responseText.Substring(0, 100) + "..." : responseText);

                    // Check if response is JSON (has content)
                    if (!string.IsNullOrEmpty(responseText) && 
                        context.Response.ContentType?.Contains("application/json") == true)
                    {
                        _logger.LogInformation("Encrypting response...");
                        
                        // Encrypt the response
                        var encryptedResponse = _payloadEncryptionService.Encrypt(responseText);
                        var encryptedBytes = Encoding.UTF8.GetBytes(encryptedResponse);

                        _logger.LogInformation("Response encrypted. Original size: {OriginalSize}, Encrypted size: {EncryptedSize}", 
                            responseText.Length, encryptedBytes.Length);

                        // Update response
                        context.Response.ContentType = "text/plain";
                        context.Response.ContentLength = encryptedBytes.Length;
                        context.Response.Body = originalBodyStream;

                        await context.Response.Body.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    else
                    {
                        _logger.LogInformation("Not encrypting response (empty or not JSON)");
                        // Copy original response back
                        context.Response.Body = originalBodyStream;
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
                else
                {
                    _logger.LogWarning("Not encrypting response (non-success status code: {StatusCode})", context.Response.StatusCode);
                    // Copy original response for non-success status codes
                    context.Response.Body = originalBodyStream;
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
