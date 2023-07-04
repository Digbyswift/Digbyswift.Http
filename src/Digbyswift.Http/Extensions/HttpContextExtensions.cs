using Microsoft.AspNetCore.Http;

namespace Digbyswift.Http.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsAuthenticated(this HttpContext httpContext)
        {
            return httpContext.User.Identity is { IsAuthenticated: true };
        }
    }
}