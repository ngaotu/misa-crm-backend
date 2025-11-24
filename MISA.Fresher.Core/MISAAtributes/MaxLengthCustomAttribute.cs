
using System;
namespace MISA.CRM.Core.MISAAtributes
{
    /// <summary>
    /// Attribute kiểm tra độ dài tối đa của chuỗi
    /// </summary>
    /// CreatedBy: NTT (15/11/2025)
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthCustomAttribute : Attribute
    {
        public int MaxLength { get; set; }
        public string? Message { get; set; }

        public MaxLengthCustomAttribute(int maxLength, string message)
        {
            MaxLength = maxLength;
            Message = message;
        }
    }
}