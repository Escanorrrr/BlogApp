using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using BlogApp.Entities.Helpers;
using BlogApp.Business.Helpers;

namespace BlogApp.Middleware
{
    public class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Endpoint'i al
            var endpoint = context.GetEndpoint();
            
            // Eğer endpoint'in Authorize özniteliği varsa kontrol et
            if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() != null)
            {
                // AdminPolicy'yi gerektiren endpointleri kontrol et
                var authAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();
                var requiresAdmin = authAttributes.Any(a => a.Policy == "AdminPolicy");
                
                // Admin yetkilendirmesi gereken bir endpoint ise ve kullanıcı kimliği doğrulanmışsa
                if (requiresAdmin && context.User.Identity.IsAuthenticated)
                {
                    // Admin kontrolü yap
                    var isAdmin = UserHelper.IsLoggedInUserAdmin(context);
                    
                    // Admin değilse erişimi reddet
                    if (!isAdmin)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        
                        var result = JsonSerializer.Serialize(
                            Result<object>.FailureResult("Bu işlemi yapmak için admin yetkiniz bulunmamaktadır.")
                        );
                        
                        await context.Response.WriteAsync(result);
                        return; // İşlemi burada sonlandır, diğer middleware'lere geçme
                    }
                }
            }

            // Sonraki middleware'e geç
            await _next(context);
        }
    }

    // Extension metodu: Program.cs'de kullanım kolaylığı için
    public static class AdminAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAuthorizationMiddleware>();
        }
    }
} 