using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Back_end_harjoitustyö.Middleware
{
    public class ApiKeyMiddleware : IMiddleware
    {
        private readonly IConfiguration _configuration;
        private const string APIKEYNAME = "ApiKey";

        public ApiKeyMiddleware(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.Value;
            if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/favicon.ico")))
            {
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key missing");
                return;
            }

            var apiKey = _configuration.GetValue<string>(APIKEYNAME);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API key");
                return;
            }
            await next(context);
        }
    }
}
