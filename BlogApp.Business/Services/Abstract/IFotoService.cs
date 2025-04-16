using BlogApp.Entities.Helpers;
using System.Threading.Tasks;

namespace BlogApp.Business.Services.Abstract
{
    public interface IFotoService
    {
        string Upload(string base64String);
        Task<Result<string>> UploadAsync(string base64String);
        string GenerateFileNameForPhoto(string type);
        string ExtractTypeFromBase64(string base64String);
        bool IsBase64String(string base64String);
    }
} 