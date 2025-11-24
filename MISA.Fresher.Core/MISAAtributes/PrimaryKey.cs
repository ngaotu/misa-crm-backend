using System;

namespace MISA.CRM.Core.MISAAtributes
{
    [AttributeUsage(AttributeTargets.Property)]
    /// <summary>
    /// Cờ báo khóa chính
    /// </summary>
    public class PrimaryKey : Attribute
    {
    }
}
