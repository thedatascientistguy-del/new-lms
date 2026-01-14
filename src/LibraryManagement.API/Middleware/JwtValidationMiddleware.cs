using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.API.Middleware
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var path = context.Request.Path.Value?.ToLower();
            
            // Skip validation for auth endpoints
            if (path.Contains("/auth/"))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Token missing");
                return;
            }

            var userId = jwtService.ValidateToken(token);
            
            if (!userId.HasValue)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid token");
                return;
            }

            // Store userId in HttpContext for use in controllers
            context.Items["UserId"] = userId.Value;

            // Validate that the userId in token matches the userId in claims
            var claimUserId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(claimUserId) && int.Parse(claimUserId) != userId.Value)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request: Tampered request detected");
                return;
            }

            await _next(context);
        }
    }
}
