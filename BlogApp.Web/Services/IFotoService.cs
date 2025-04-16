using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BlogApp.Web.Services
{
    public interface IFotoService
    {
        Task<string> UploadAsync(IFormFile file);
    }
} 