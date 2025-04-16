using BlogApp.Business.Services.Abstract;
using BlogApp.DataAccess.Context;
using System;
using System.IO;
using System.Text.RegularExpressions;
using BlogApp.Entities.Enums;
using System.Threading.Tasks;
using BlogApp.Entities.Helpers;

namespace BlogApp.Business.Services.Concrete
{
    public class FotoService : IFotoService
    {
        private readonly BlogDbContext _context;
        private readonly string _uploadPath;

        public FotoService(BlogDbContext context)
        {
            _context = context;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }
        
        public string Upload(string base64String)
        {
            try
            {
                if (!IsBase64String(base64String))
                {
                    return null;
                }

                var fileType = ExtractTypeFromBase64(base64String);
                if (string.IsNullOrEmpty(fileType))
                {
                    return null;
                }

                var fileName = GenerateFileNameForPhoto(fileType);
                var filePath = Path.Combine(_uploadPath, fileName);

                // Base64'ü byte dizisine çevir
                var base64Data = base64String.Split(',')[1];
                var imageBytes = Convert.FromBase64String(base64Data);

                // Dosyayı kaydet
                File.WriteAllBytes(filePath, imageBytes);

                return $"/images/{fileName}"; // /images/dosya.jpg formatında dön
            }
            catch (Exception ex)
            {
                // Hata loglanabilir
                return null;
            }
        }

        public string GenerateFileNameForPhoto(string type)
        {
            string uniqueFileName = Guid.NewGuid().ToString();
            return uniqueFileName + (!string.IsNullOrEmpty(type) ? type : ".jpg");
        }

        public string ExtractTypeFromBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || !base64String.Contains("data:"))
                return ".jpg";

            try
            {
                return "." + base64String.Split(',')[0]
                    .Split(':')[1]
                    .Split(';')[0]
                    .Split('/')[1];
            }
            catch
            {
                return ".jpg";
            }
        }
        
        public bool IsBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return false;

            if (!base64String.Contains("data:image/") || !base64String.Contains(";base64,"))
                return false;

            try
            {
                var base64Data = base64String.Split(',')[1];
                Convert.FromBase64String(base64Data);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Result<string>> UploadAsync(string base64String)
        {
            try
            {
                if (!IsBase64String(base64String))
                {
                    return Result<string>.FailureResult("Geçersiz resim formatı.");
                }

                var fileType = ExtractTypeFromBase64(base64String);
                if (string.IsNullOrEmpty(fileType))
                {
                    return Result<string>.FailureResult("Resim türü belirlenemedi.");
                }

                var fileName = GenerateFileNameForPhoto(fileType);
                var filePath = Path.Combine(_uploadPath, fileName);

                // Base64'ü byte dizisine çevir
                var base64Data = base64String.Split(',')[1];
                var imageBytes = Convert.FromBase64String(base64Data);

                // Dosyayı kaydet
                await File.WriteAllBytesAsync(filePath, imageBytes);

                return Result<string>.SuccessResult($"/images/{fileName}"); // /images/dosya.jpg formatında dön
            }
            catch (Exception ex)
            {
                return Result<string>.FailureResult($"Resim yüklenirken bir hata oluştu: {ex.Message}");
            }
        }
    }
} 