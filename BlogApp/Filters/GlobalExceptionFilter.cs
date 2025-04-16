using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace BlogApp.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            // Hatayı logla (detaylı bilgiyle)
            _logger.LogError(context.Exception, "Beklenmeyen bir hata oluştu: {ErrorMessage}", context.Exception.Message);

            // Cevabı oluştur - kullanıcıya detaylı hata mesajı gösterme
            var response = new
            {
                Success = false,
                Message = "İşlem sırasında bir hata oluştu.",
                // Dev ortamında bile detayları gösterme
                ErrorCode = context.Exception.GetType().Name
            };

            // HTTP yanıtını oluştur
            context.Result = new JsonResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            // Exception'ın işlendiğini belirt
            context.ExceptionHandled = true;
        }
    }
} 