using BlogApp.Entities.Enums;
using BlogApp.Entities.Helpers;
using System.Threading.Tasks;

namespace BlogApp.Business.Services.Abstract
{
    public interface IFotoService
    {
        string Upload(string base64String, PhotoType photoType);
        string GenerateFileNameForPhoto(string type);
        string ExtractTypeFromBase64(string base64String);
        bool IsBase64String(string base64String);
    }
} 