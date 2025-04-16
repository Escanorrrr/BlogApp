using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlogApp.Web.Services
{
    public class FotoService : IFotoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FotoService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Dosya bulunamadı", nameof(file));
            }

            var client = _httpClientFactory.CreateClient("BlogApi");

            using var content = new MultipartFormDataContent();
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            content.Add(new ByteArrayContent(stream.ToArray()), "file", file.FileName);

            var response = await client.PostAsync("api/Foto/Upload", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
} 