namespace MISA.CRM.Core.DTOs.Common
{
    /// <summary>
    /// DTO chứa kết quả phân trang
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu các items</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Danh sách dữ liệu trong trang
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        /// <summary>
        /// Có trang trước không
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Có trang sau không
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
    }
}