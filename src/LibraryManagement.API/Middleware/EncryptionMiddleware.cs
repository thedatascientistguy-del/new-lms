using LibraryManagement.Core.Interfaces;
using System.Text;

namespace LibraryManagement.API.Middleware
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPayloadEncryptionService _payloadEncryptionService;

        public EncryptionMiddleware(RequestDelegate next, IPayloadEncryptionService payloadEncryptionService)
        {
            _next = next;
            _payloadEncryptionService = payloadEncryptionService;
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

                // Only encrypt successful responses (200-299)
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    // Read the response
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    // Check if response is JSON (has content)
                    if (!string.IsNullOrEmpty(responseText) && 
                        context.Response.ContentType?.Contains("application/json") == true)
                    {
                        // Encrypt the response
                        var encryptedResponse = _payloadEncryptionService.Encrypt(responseText);
                        var encryptedBytes = Encoding.UTF8.GetBytes(encryptedResponse);

                        // Update response
                        context.Response.ContentType = "text/plain";
                        context.Response.ContentLength = encryptedBytes.Length;
                        context.Response.Body = originalBodyStream;

                        await context.Response.Body.WriteAsync(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    else
                    {
                        // Copy original response back
                        context.Response.Body = originalBodyStream;
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
                else
                {
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
