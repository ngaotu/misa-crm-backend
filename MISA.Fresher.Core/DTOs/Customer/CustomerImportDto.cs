namespace MISA.CRM.Core.DTOs.Customer
{
    /// <summary>
    /// DTO cho import khách hàng từ CSV
    /// Mapping các cột tiếng Việt từ file CSV
    /// </summary>
    /// CreatedBy: NTT (19/11/2025)
    public class CustomerImportDto
    {
        /// <summary>
        /// Số dòng trong file CSV (để báo lỗi)
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Họ và tên khách hàng - mapping từ cột "Họ và tên"
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Số điện thoại - mapping từ cột "Số điện thoại"  
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Email - mapping từ cột "Email"
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Địa chỉ - mapping từ cột "Địa chỉ"
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Loại khách hàng - mapping từ cột "Loại khách hàng"
        /// </summary>
        public string? CustomerType { get; set; }


        /// <summary>
        /// Mã số thuế - mapping từ cột "Mã số thuế"
        /// </summary>
        public string? TaxCode { get; set; }

        /// <summary>
        /// Ngày mua gần nhất - mapping từ cột "Ngày mua gần nhất"
        /// </summary>
        public DateTime? LastPurchaseDate { get; set; }

        /// <summary>
        /// Hàng hóa đã mua - mapping từ cột "Hàng hóa đã mua"
        /// </summary>
        public string? PurchasedItems { get; set; }

        /// <summary>
        /// Hàng hóa mua gần nhất - mapping từ cột "Hàng hóa mua gần nhất"
        /// </summary>
        public string? LatestPurchasedItems { get; set; }
    }
}