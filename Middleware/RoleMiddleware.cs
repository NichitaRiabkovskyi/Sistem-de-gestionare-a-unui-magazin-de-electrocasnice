using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MyApi.Middleware
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;
        public RoleMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // Citim rolul din headerul X-User-Role (poți schimba cu JWT în viitor)
            var role = context.Request.Headers["X-User-Role"].FirstOrDefault();
            if (!string.IsNullOrEmpty(role))
                context.Items["UserRole"] = role;
            else
                context.Items["UserRole"] = "Anonymous";

            var path = context.Request.Path.Value ?? "";

            // Regula ceruta: orice /admin/* -> doar Admin. Altfel 403.
            if (path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "Forbidden: Admins only." });
                    return;
                }
            }

            // Exemplu: rapoarte accesibile doar Admin (implementat de mai sus)
            await _next(context);
        }
    }
}
