using MISA.CRM.Core.DTOs.Common;

namespace MISA.CRM.Core.DTOs.Customer
{
    /// <summary>
    /// DTO cho yêu c?u tìm ki?m và l?c nhanh khách hàng
    /// G?p t?t c? tính n?ng: phân trang + s?p x?p + l?c nhanh
    /// </summary>
    /// CreatedBy: NTT (15/11/2025)
    public class CustomerFilterRequest : PagedRequest
    {
        /// <summary>
        /// L?c theo tên khách hàng (LIKE search)
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// L?c theo email khách hàng (LIKE search)
        /// </summary>
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// L?c theo s? ?i?n tho?i khách hàng (LIKE search)
        /// </summary>
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// L?c theo lo?i khách hàng (Exact match)
        /// </summary>
        public string? CustomerType { get; set; }

        /// <summary>
        /// L?c theo mã khách hàng (LIKE search)
        /// </summary>
        public string? CustomerCode { get; set; }
    }
}