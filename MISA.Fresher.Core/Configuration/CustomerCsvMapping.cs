using MISA.CRM.Core.Entities;

namespace MISA.CRM.Core.Configuration
{
    /// <summary>
    /// Configuration mapping cho Import/Export CSV Customer
    /// </summary>
    public static class CustomerCsvMapping
    {
        /// <summary>
        /// Mapping cột CSV tiếng Việt sang Property của Customer Entity
        /// Key: Tên cột trong file CSV (tiếng Việt)
        /// Value: Property name trong Customer class
        /// </summary>
        /// CreatedBy: NTT (19/11/2025)
        public static readonly Dictionary<string, string> ColumnMapping = new()
        {
            // Tất cả cột đều là optional - KHÔNG có cột bắt buộc
            { "Họ và tên", nameof(Customer.CustomerFullName) },
            { "Số điện thoại", nameof(Customer.CustomerPhone) },
            { "Email", nameof(Customer.CustomerEmail) },
            { "Địa chỉ", nameof(Customer.CustomerShippingAddr) },
            { "Loại khách hàng", nameof(Customer.CustomerType) },
            { "Mã khách hàng", nameof(Customer.CustomerCode) },
            { "Mã số thuế", nameof(Customer.CustomerTaxCode) },
            { "Ngày mua gần nhất", nameof(Customer.CustomerLastPurchaseDate) },
            { "Hàng hóa đã mua", nameof(Customer.CustomerPurchasedItems) },
            { "Hàng hóa mua gần nhất", nameof(Customer.CustomerLastestPurchasedItems) }
        };

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
        /// Mapping Property của Customer Entity sang tên cột Export CSV
        /// </summary>
        public static readonly Dictionary<string, string> PropertyToExportColumn = new()
        {
            { nameof(Customer.CustomerCode), "Mã khách hàng" },
            { nameof(Customer.CustomerFullName), "Họ và tên" },
            { nameof(Customer.CustomerEmail), "Email" },
            { nameof(Customer.CustomerPhone), "Số điện thoại" },
            { nameof(Customer.CustomerType), "Loại khách hàng" },
            { nameof(Customer.CustomerShippingAddr), "Địa chỉ" },
            { nameof(Customer.CustomerTaxCode), "Mã số thuế" },
            { nameof(Customer.CustomerLastPurchaseDate), "Ngày mua gần nhất" },
            { nameof(Customer.CustomerPurchasedItems), "Hàng hóa đã mua" },
            { nameof(Customer.CustomerLastestPurchasedItems), "Hàng hóa mua gần nhất" }
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
        /// CreatedBy: NTT (19/11/2025)
        public static bool IsValidCustomerType(string? customerType)
        {
            return string.IsNullOrWhiteSpace(customerType) ||
                   AllowedCustomerTypes.Contains(customerType.Trim());
        }

        /// <summary>
        /// Lấy tên cột CSV từ property name
        /// </summary>
        /// <param name="propertyName">Tên property trong entity (ví dụ: CustomerEmail)</param>
        /// <returns>Tên cột CSV tương ứng nếu mapping tồn tại, ngược lại trả về propertyName</returns>
        /// CreatedBy: NTT (19/11/2025)
        public static string GetCsvColumnName(string propertyName)
        {
            return PropertyToExportColumn.TryGetValue(propertyName, out var columnName)
                ? columnName
                : propertyName;
        }

        /// <summary>
        /// Lấy property name từ tên cột CSV
        /// </summary>
        /// <param name="csvColumnName">Tên cột trong file CSV (tiếng Việt)</param>
        /// <returns>Tên property tương ứng nếu mapping tồn tại, ngược lại trả về csvColumnName</returns>
        /// CreatedBy: NTT (19/11/2025)
        public static string GetPropertyName(string csvColumnName)
        {
            return ColumnMapping.TryGetValue(csvColumnName, out var propertyName)
                ? propertyName
                : csvColumnName;
        }
    }
}