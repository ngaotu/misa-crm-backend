using System;

namespace MISA.CRM.Core.MISAAtributes
{
    [AttributeUsage(AttributeTargets.Class)]
    /// <summary>
    /// Cờ báo tên table
    /// </summary>
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// Tên bảng
        /// </summary>
        public string? Name { get; set; }
        public TableNameAttribute(string? name)
        {
            Name = name;
        }
    }
}
