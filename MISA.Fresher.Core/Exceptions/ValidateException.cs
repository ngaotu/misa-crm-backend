using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM.Core.Exceptions
{
    /// <summary>
    /// Ngoại lệ được ném ra khi dữ liệu không hợp lệ (validation thất bại).
    /// </summary>
    public class ValidateException : Exception
    {
        /// <summary>
        /// Khởi tạo ValidationException với thông điệp cụ thể.
        /// </summary>
        /// <param name="message">Thông điệp lỗi.</param>
        public ValidateException(string message) : base(message)
        {
        }
    }
}
