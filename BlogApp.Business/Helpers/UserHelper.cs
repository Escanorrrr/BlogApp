using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Business.Helpers
{
    public static class UserHelper
    {
        /// <summary>
        /// JWT token ile gelen kullanıcının admin olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Kullanıcı admin ise true, değilse false</returns>
        public static bool IsLoggedInUserAdmin(HttpContext httpContext)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            // Admin claim'i kontrol et (büyük-küçük harf duyarsız)
            return httpContext.User.HasClaim(c => 
                (c.Type.Equals("isAdmin", StringComparison.OrdinalIgnoreCase) && 
                (c.Value.Equals("True", StringComparison.OrdinalIgnoreCase) || c.Value.Equals("true")))
            );
        }

        /// <summary>
        /// JWT token ile gelen kullanıcının ID'sini döndürür.
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Kullanıcı ID'si, eğer kullanıcı doğrulanmamışsa veya claim yoksa null</returns>
        public static int? GetLoggedInUserId(HttpContext httpContext)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
} 