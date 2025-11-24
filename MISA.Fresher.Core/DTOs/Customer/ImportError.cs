using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM.Core.DTOs.Customer
{
    /// <summary>
    /// Thông tin lỗi khi import
    /// </summary>
    /// CreatedBy: NTT (19/11/2025)
    public class ImportError
    {
        /// <summary>
        /// Số dòng bị lỗi
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Thông báo lỗi
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu gốc dòng lỗi
        /// </summary>
        public CustomerImportDto OriginalRow { get; set; } = new CustomerImportDto();

    }
}
