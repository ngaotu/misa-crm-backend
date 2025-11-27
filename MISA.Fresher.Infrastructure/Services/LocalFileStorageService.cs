using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using MISA.CRM.Core.Interfaces.Services;
using MISA.CRM.Core.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MISA.CRM.Infrastructure.Services
{
    /// <summary>
    /// Triển khai lưu file cục bộ sử dụng FileStorageOptions (không phụ thuộc IWebHostEnvironment).
    /// Lớp này thuộc tầng Infrastructure và implement IFileStorageService (Core contract).
    /// </summary>
    /// <remarks>CreatedBy: NTT (21/11/2025)</remarks>
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;

        /// <summary>
        /// Khởi tạo LocalFileStorageService với IOptions<FileStorageOptions>
        /// </summary>
        /// <param name="options">Options chứa cấu hình RootPath</param>
        public LocalFileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Lấy đường dẫn root thực tế từ cấu hình
        /// Nếu RootPath là đường dẫn tương đối sẽ quy về thư mục chạy ứng dụng (ContentRoot)
        /// </summary>
        /// CreatedBy: NTT (21/11/2025)
        private string GetRootPath()
        {
            var root = _options.RootPath ?? "wwwroot";
            if (Path.IsPathRooted(root)) return root;
            return Path.Combine(Directory.GetCurrentDirectory(), root);
        }

        /// <summary>
        /// Lưu file vào thư mục `folder` bên trong RootPath
        /// Trả về đường dẫn relative để client sử dụng (ví dụ: /uploads/avatars/xxx.png)
        /// </summary>
        /// <param name="file">File upload từ client</param>
        /// <param name="folder">Thư mục tương đối bên trong root (ví dụ: "uploads/avatars")</param>
        /// <returns>Relative url hoặc null nếu file null</returns>
        /// CreatedBy: NTT (21/11/2025)
        public string? SaveFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            var uploadsRoot = GetRootPath();
            var targetFolder = Path.Combine(uploadsRoot, folder.Trim('\'', '/'));
            Directory.CreateDirectory(targetFolder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var relativeUrl = $"/{folder.Trim('\'', '/')}/{fileName}".Replace("\\", "/");
            return relativeUrl;
        }

        /// <summary>
        /// Xóa file theo đường dẫn relative
        /// </summary>
        /// <param name="relativePath">Đường dẫn relative (ví dụ: /uploads/avatars/xxx.png)</param>
        /// CreatedBy: NTT (21/11/2025)
        public void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var uploadsRoot = GetRootPath();
            var fullPath = Path.Combine(uploadsRoot, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}
