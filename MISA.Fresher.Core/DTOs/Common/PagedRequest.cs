using System.ComponentModel.DataAnnotations;

namespace MISA.CRM.Core.DTOs.Common
{
    /// <summary>
    /// DTO chung cho các yêu cầu phân trang
    /// </summary>
    public class PagedRequest
    {

        /// <summary>
        ///  Trang hiện tại
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        [Range(1, 100, ErrorMessage = "Kích thước trang phải nhỏ hơn 100")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Cột sắp xếp
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Hướng sắp xếp (asc/desc)
        /// </summary>
        public string SortDirection { get; set; } = "asc";

        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string? Search { get; set; }
    }
}