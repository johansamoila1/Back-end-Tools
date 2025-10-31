using Back_end_harjoitustyö.Services;

namespace Back_end_harjoitustyö.Middleware
{
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path.StartsWith("/swagger") ||
                path.StartsWith("/api/users/login") ||
                path.StartsWith("/api/users/register") ||
                path.StartsWith("/api/users/seed"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing or invalid token");
                return;
            }

            var userId = TokenService.ValidateToken(token!);
            if (userId == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token");
                return;
            }

            context.Items["UserId"] = userId.Value;
            await _next(context);
        }
    }

    public static class TokenAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthMiddleware>();
        }
    }
}
