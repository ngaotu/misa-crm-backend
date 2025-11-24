using System;

namespace MISA.CRM.Core.MISAAtributes
{
    /// <summary>
    /// Attribute đánh dấu property cần đảm bảo unique (không được trùng trong DB)
    /// </summary>
    /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueAttribute : Attribute
    {
        /// <summary>
        /// Thông điệp lỗi tuỳ chỉnh khi phát hiện trùng
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Khởi tạo UniqueAttribute
        /// </summary>
        /// <param name="message">Thông điệp lỗi tuỳ chọn</param>
        public UniqueAttribute(string? message = null)
        {
            Message = message;
        }
    }
}
