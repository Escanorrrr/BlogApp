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
        private readonly string _blogPhotosPath;
        private readonly string _userPhotosPath;

        public FotoService(BlogDbContext context)
        {
            _context = context;
            
            // bin/Debug/net8.0'dan solution dizinine git (4 klasör yukarı)
            var solutionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..")); 
            
            // Web projesinin wwwroot/images dizinini bul
            var webRootPath = Path.Combine(solutionPath, "BlogApp.Web", "wwwroot", "images");
            
            // Blog ve kullanıcı fotoğrafları için dizinleri ayarla
            _blogPhotosPath = Path.Combine(webRootPath, "blog_photos");
            _userPhotosPath = Path.Combine(webRootPath, "user_photos");
            
            // Dizinlerin varlığını kontrol et ve yoksa oluştur
            if (!Directory.Exists(webRootPath))
                Directory.CreateDirectory(webRootPath);
            if (!Directory.Exists(_blogPhotosPath))
                Directory.CreateDirectory(_blogPhotosPath);
            if (!Directory.Exists(_userPhotosPath))
                Directory.CreateDirectory(_userPhotosPath);
        }

        public string Upload(string base64String, PhotoType photoType)
        {
            //Metadata'yı temizle
            var base64Data = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

            //Base64 data'ı byte dizisine çevir (byte array bir fotoğrafa karşılık gelir)
            byte[] bytes = Convert.FromBase64String(base64Data);

            //Base 64 stringten uzantıyı çıkart.
            string type = ExtractTypeFromBase64(base64String);

            //Fotoğraf dosya adı oluştur (UUID)
            string fileName = GenerateFileNameForPhoto(type);

            //Fotoğrafın kaydedileceği dizin - fotoğraf tipine göre değişir
            string targetDirectory = photoType == PhotoType.BLOG_PHOTO ? _blogPhotosPath : _userPhotosPath;

            //Fotoğrafın tam dosya yolu
            string filePath = Path.Combine(targetDirectory, fileName);

            //Klasörün varlığını kontrol et
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            // Dosyayı yaz
            File.WriteAllBytes(filePath, bytes);
            //Filepath return edilir bu sayede dosyanın filepath bilgisi dönülmüş olur.
            return fileName;
        }

        public string GenerateFileNameForPhoto(string type)
        {
            string uniqueFileName = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(type))
            {
                type = ".jpg"; // Varsayılan uzantı
            }

            return uniqueFileName + type;
        }

        public string ExtractTypeFromBase64(string base64String)
        {
            // MIME türünden dosya uzantısını al
            string fileExtension = base64String.Contains("data:")
                ? "." + base64String.Split(',')[0] // "data:image/jpeg;base64"
                    .Split(':')[1]                // "image/jpeg;base64"
                    .Split(';')[0]                // "image/jpeg"
                    .Split('/')[1]                // "jpeg"
                : ".jpg"; // Eğer bir tür bulunamazsa varsayılan .jpg dön

            return fileExtension;
        }

        public bool IsBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return false;

            // Base64 görüntü formatına benziyor mu temel kontrol
            if (base64String.Contains("data:image/") && base64String.Contains(";base64,"))
            {
                // Basit bir doğrulama denemesi
                try
                {
                    // Base64 kısmını ayır
                    var base64Data = base64String.Split(',')[1];
                    // Base64 string'i byte array'e çevirmeyi dene
                    Convert.FromBase64String(base64Data);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

     
    }
} 