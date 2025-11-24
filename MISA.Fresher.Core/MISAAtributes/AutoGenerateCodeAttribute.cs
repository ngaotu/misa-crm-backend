using System;

namespace MISA.CRM.Core.MISAAtributes
{
    /// <summary>
    /// Attribute đánh dấu property cần tự động sinh mã theo format: prefix + yyyyMM + 6 số tăng dần
    /// Ví dụ: [AutoGenerateCode("KH")] cho Customer
    /// </summary>
    /// Created By: NTT (15/11/2025)
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoGenerateCodeAttribute : Attribute
    {
        /// <summary>
        /// Tiền tố của mã (KH, NV, SP...)
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Khởi tạo AutoGenerateCodeAttribute với prefix
        /// </summary>
        /// <param name="prefix">Tiền tố (KH, NV, SP...)</param>
        public AutoGenerateCodeAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}
