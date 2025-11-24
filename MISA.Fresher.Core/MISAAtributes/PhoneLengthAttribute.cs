using System;

namespace MISA.CRM.Core.MISAAtributes
{
    /// <summary>
    /// Attribute kiểm tra số điện thoại có 10-11 chữ số
    /// </summary>
    /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class PhoneLengthAttribute : Attribute
    {
        /// <summary>
        /// Thông điệp lỗi tuỳ chỉnh
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Khởi tạo PhoneLengthAttribute với thông điệp lỗi
        /// </summary>
        /// <param name="message">Thông điệp lỗi</param>
        public PhoneLengthAttribute(string message)
        {
            Message = message;
        }
    }
}