using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM.Core.Exceptions
{
    /// <summary>
    /// Ngoại lệ được ném ra khi có xung đột dữ liệu (ví dụ trùng key).
    /// </summary>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Khởi tạo ConflictException với thông điệp cụ thể.
        /// </summary>
        /// <param name="message">Thông điệp lỗi.</param>
        public ConflictException(string message) : base(message)
        {
        }
    }
}
