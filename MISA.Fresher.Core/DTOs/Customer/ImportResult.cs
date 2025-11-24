namespace MISA.CRM.Core.DTOs.Customer
{
    /// <summary>
    /// Kết quả import CSV
    /// </summary>
    /// CreatedBy: NTT (19/11/2025)
    public class ImportResult
    {
        /// <summary>
        /// Tổng số dòng trong file CSV
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Số bản ghi import thành công
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// Số bản ghi bị lỗi
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Danh sách lỗi chi tiết
        /// </summary>
        public List<ImportError> Errors { get; set; } = new List<ImportError>();

        /// <summary>
        /// Tóm tắt kết quả
        /// </summary>
        public string Summary => $"Thành công: {SuccessCount}/{TotalRows}. Lỗi: {ErrorCount} bản ghi.";
    }

   
}