using Microsoft.AspNetCore.Http;

namespace RealStateApp.Helpers
{
    public static class FileHelper
    {
        public static string Upload(IFormFile file, int EntityId, string folderName, string imgPath = "", bool isEditMode = false)
        {
            if (isEditMode && file == null) return imgPath;
            if (file == null) return string.Empty;
            
            string basePath = $"/Images/{folderName}/{EntityId}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileName = guid + fileInfo.Extension;
            string fullFilePath = Path.Combine(path, fileName);
            
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            
            return $"{basePath}/{fileName}";
        }

        public static string UploadWithoutId(IFormFile file, string folderName, string imgPath = "", bool isEditMode = false)
        {
            if (isEditMode && file == null) return imgPath;
            if (file == null) return string.Empty;
            
            string basePath = $"/Images/{folderName}";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Guid guid = Guid.NewGuid();
            FileInfo fileInfo = new FileInfo(file.FileName);
            string fileName = guid + fileInfo.Extension;
            string fullFilePath = Path.Combine(path, fileName);
            
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            
            return $"{basePath}/{fileName}";
        }

        public static async Task<byte[]> ConvertFileToBytesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{filePath}");
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
