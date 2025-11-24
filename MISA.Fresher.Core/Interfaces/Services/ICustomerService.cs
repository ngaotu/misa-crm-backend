using System;
using System.Collections.Generic;
using MISA.CRM.Core.Entities;
using MISA.CRM.Core.DTOs.Customer;
using MISA.CRM.Core.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace MISA.CRM.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho nghiệp vụ khách hàng
    /// </summary>
    /// Created By: NTT (15/11/2025)
    public interface ICustomerService : IBaseService<Customer>
    {
        /// <summary>
        /// Lấy danh sách khách hàng có phân trang + sắp xếp + lọc + tìm kiếm
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm chung</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi/trang</param>
        /// <param name="sortBy">Cột sắp xếp</param>
        /// <param name="sortDirection">Hướng sắp xếp</param>
        /// <returns>Kết quả phân trang khách hàng</returns>
        /// CreatedBy: NTT (16/11/2025)
        PagedResult<Customer> GetPagedCustomers(string? search = null, int page = 1, int pageSize = 10, string? sortBy = null, string? sortDirection = null);

        /// <summary>
        /// Export danh sách khách hàng được chọn ra CSV (checkbox/bulk action)
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng cần export</param>
        /// <returns>File CSV dưới dạng byte array</returns>
        /// CreatedBy: NTT (19/11/2025)
        byte[] ExportCustomersToCSV(List<Guid> customerIds);

        /// <summary>
        /// Import khách hàng từ file CSV  
        /// Áp dụng Partial Success Strategy - bản ghi hợp lệ được lưu, bản ghi lỗi báo chi tiết
        /// </summary>
        /// <param name="csvFile">File CSV upload</param>
        /// <returns>Kết quả import với thống kê và danh sách lỗi</returns>
        /// CreatedBy: NTT (19/11/2025)
        ImportResult ImportCustomersFromCSV(IFormFile csvFile);

        /// <summary>
        /// Gán loại khách hàng cho nhiều khách hàng theo danh sách ID.
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng</param>
        /// <param name="customerType">Loại khách hàng cần gán</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: NTT (16/11/2025)
        int AssignCustomerType(List<Guid> customerIds, string customerType);

        /// <summary>
        /// Xóa mềm nhiều khách hàng theo danh sách ID.
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: NTT (16/11/2025)
        int BulkDelete(List<Guid> customerIds);

        /// <summary>
        /// Kiểm tra xem email đã tồn tại trong hệ thống hay chưa.
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="excludeId">ID của khách hàng</param>
        /// <returns>True nếu email đã tồn tại, ngược lại false</returns>
        /// CreatedBy: NTT (18/11/2025)
        bool ExistsByEmail(string email, Guid? excludeId = null);

        /// <summary>
        /// Kiểm tra xem số điện thoại đã tồn tại trong hệ thống hay chưa.
        /// </summary>
        /// <param name="phone">Số điện thoại cần kiểm tra</param>
        /// <param name="excludeId">ID của khách hàng</param>
        /// <returns>True nếu số điện thoại đã tồn tại, ngược lại false</returns>
        /// CreatedBy: NTT (18/11/2025)
        bool ExistsByPhone(string phone, Guid? excludeId = null);
    }
}
