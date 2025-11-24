using System;
using System.Collections.Generic;
using MISA.CRM.Core.Entities;
using MISA.CRM.Core.DTOs.Common;

namespace MISA.CRM.Core.Interfaces.Repositories
{
    public interface ICustomerRepository : IBaseRepository<Customer>
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
        /// Lấy danh sách khách hàng theo list ID (cho export bulk)
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng</param>
        /// <returns>Danh sách khách hàng</returns>
        /// CreatedBy: NTT (19/11/2025)
        List<Customer> GetByIds(List<Guid> customerIds);
    }
}
