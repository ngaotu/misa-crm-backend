using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MISA.CRM.Core.Interfaces.Services
{
    /// <summary>
    /// Interface định nghĩa các thao tác lưu trữ file (upload/delete) sử dụng bởi ứng dụng.
    /// </summary>
    /// CreatedBy: NTT (21/11/2025)
    public interface IFileStorageService
    {
        /// <summary>
        /// Lưu file lên storage local theo folder chỉ định.
        /// </summary>
        /// <param name="file">File upload (IFormFile) từ client</param>
        /// <param name="folder">Thư mục tương đối trong root storage (ví dụ: "uploads/avatars")</param>
        /// <returns>Đường dẫn relative (ví dụ: "/uploads/avatars/{filename}") hoặc null nếu không có file</returns>
        /// CreatedBy: NTT (21/11/2025)
        Task<string?> SaveFileAsync(IFormFile file, string folder);

        /// <summary>
        /// Xóa file theo đường dẫn relative trả về trước đó (relative path).
        /// </summary>
        /// <param name="relativePath">Đường dẫn tương đối bắt đầu bằng '/' hoặc không (ví dụ: "/uploads/avatars/abc.png")</param>
        /// <returns>Task hoàn thành khi xóa xong</returns>
        /// CreatedBy: NTT (21/11/2025)
        Task DeleteFileAsync(string relativePath);
    }
}
