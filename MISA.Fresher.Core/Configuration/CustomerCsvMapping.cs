using System.Collections.Generic;

namespace MISA.CRM.Core.Configuration
{
    /// <summary>
    /// Configuration mapping cho Import/Export CSV Customer
    /// </summary>
    public static class CustomerCsvMapping
    {
        /// <summary>
        /// Header cho Export CSV (thứ tự hiển thị từ trái qua phải)
        /// </summary>
        public static readonly List<string> ExportHeaders = new()
        {
            "Mã khách hàng",
            "Họ và tên",
            "Email",
            "Số điện thoại",
            "Loại khách hàng",
            "Địa chỉ",
            "Mã số thuế",
            "Ngày mua gần nhất",
            "Hàng hóa đã mua",
            "Hàng hóa mua gần nhất"
        };

        /// <summary>
        /// Các loại khách hàng được phép (để validate khi import)
        /// </summary>
        public static readonly HashSet<string> AllowedCustomerTypes = new()
        {
            "VIP",
            "NBH01",
            "LKHA"
        };

        /// <summary>
        /// Validate CustomerType có hợp lệ không
        /// </summary>
        /// <param name="customerType">Giá trị CustomerType từ file CSV</param>
        /// <returns>True nếu hợp lệ hoặc null/empty, ngược lại false</returns>
        public static bool IsValidCustomerType(string? customerType)
        {
            return string.IsNullOrWhiteSpace(customerType) ||
                   AllowedCustomerTypes.Contains(customerType.Trim());
        }
    }
}