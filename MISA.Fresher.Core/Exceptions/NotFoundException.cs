using System;

namespace MISA.CRM.Core.Exceptions
{
    /// <summary>
    /// Ngoại lệ được ném ra khi không tìm thấy tài nguyên.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Khởi tạo NotFoundException với thông điệp cụ thể.
        /// </summary>
        /// <param name="message">Thông điệp lỗi.</param>
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
