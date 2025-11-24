using System;

namespace MISA.CRM.Core.MISAAtributes
{
    /// <summary>
    /// Attribute kiểm tra định dạng email hợp lệ
    /// </summary>
    /// Created By: NTT (15/11/2025)
    [AttributeUsage(AttributeTargets.Property)]
    public class EmailFormatAttribute : Attribute
    {
        public string? Message { get; set; }

        public EmailFormatAttribute(string message)
        {
            Message = message;
        }
    }
}