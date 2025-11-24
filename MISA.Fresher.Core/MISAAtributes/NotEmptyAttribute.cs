using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM.Core.MISAAtributes
{
    /// <summary>
    /// Dùng để đánh dấu các property bắt buộc phải có dữ liệu (không được để trống)
    /// </summary>
    /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotEmptyAttribute : Attribute
    {
        /// <summary>
        /// Thông điệp lỗi tuỳ chỉnh khi property rỗng
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Khởi tạo NotEmptyAttribute với thông điệp lỗi
        /// </summary>
        /// <param name="message">Thông điệp lỗi</param>
        public NotEmptyAttribute(string message)
        {
            Message = message;
        }
    }

}
