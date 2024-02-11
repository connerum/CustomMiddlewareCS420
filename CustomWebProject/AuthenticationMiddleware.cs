using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CustomWebProject
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var username = context.Request.Query["username"];
            var password = context.Request.Query["password"];

            if (username != "user1" || password != "password1")
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Not authorized.");
                return;
            }

            await _next(context);
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
