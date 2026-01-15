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
            if (path != null && path.Contains("/auth/"))
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

            await _next(context);
        }
    }
}
