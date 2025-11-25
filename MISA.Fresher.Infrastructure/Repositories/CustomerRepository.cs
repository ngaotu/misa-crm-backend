using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MISA.CRM.Core.Entities;
using MISA.CRM.Core.Interfaces.Repositories;
using MISA.CRM.Core.DTOs.Customer;
using MISA.CRM.Core.DTOs.Common;
using Dapper;
using System.Data;

namespace MISA.CRM.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        ///  Lấy danh sách khách hàng có phân trang + sắp xếp + tìm kiếm chung
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm chung</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi/trang</param>
        /// <param name="sortBy">Cột sắp xếp</param>
        /// <param name="sortDirection">Hướng sắp xếp</param>
        /// <returns>Kết quả phân trang khách hàng</returns>
        /// CreatedBy: NTT(16/11/2025)
        public PagedResult<Customer> GetPagedCustomers(string? search = null, int page = 1, int pageSize = 10, string? sortBy = null, string? sortDirection = null)
        {
            using (var multi = dbConnection.QueryMultiple(
                "proc_customers_paging_and_sort",
                new
                {
                    p_query = search,
                    p_page_number = page,
                    p_page_size = pageSize,
                    p_sort_by = sortBy ?? "CustomerCode",
                    p_sort_direction = sortDirection ?? "DESC"
                },
                commandType: CommandType.StoredProcedure
            ))
            {
                var customers = multi.Read<Customer>().ToList();
                var totalResult = multi.ReadSingle<dynamic>();
                var totalRecords = (int)totalResult.TotalRecords;

                return new PagedResult<Customer>
                {
                    Items = customers,
                    TotalRecords = totalRecords,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
        }

        /// <summary>
        /// Phân loại khách hàng theo danh sách Id
        /// </summary>
        /// <param name="customerIds">Danh sách Id khách hàng</param>
        /// <param name="customerType">Loại khách hàng</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: NTT (16/11/2025)
        public int AssignCustomerType(List<Guid> customerIds, string customerType)
        {
            var sql = "UPDATE customer SET customer_type = @CustomerType WHERE customer_id IN @Ids AND is_deleted =0";
            return dbConnection.Execute(sql, new { CustomerType = customerType, Ids = customerIds });
        }

        /// <summary>
        /// Xóa nhiều khách hàng theo danh sách Id (các bản ghi sẽ được đánh dấu là đã xóa)
        /// </summary>
        /// <param name="customerIds">Danh sách Id khách hàng</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: NTT (16/11/2025)
        public int BulkDelete(List<Guid> customerIds)
        {
            var sql = "UPDATE customer SET is_deleted =1 WHERE customer_id IN @Ids AND is_deleted =0";
            return dbConnection.Execute(sql, new { Ids = customerIds });
        }

        /// <summary>
        /// Lấy danh sách khách hàng theo danh sách Id
        /// </summary>
        /// <param name="customerIds">Danh sách Id khách hàng</param>
        /// <returns>Danh sách khách hàng</returns>
        /// CreatedBy: NTT (16/11/2025)
        public List<Customer> GetByIds(List<Guid> customerIds)
        {
            if (customerIds == null || customerIds.Count == 0) return new List<Customer>();
            var sql = "SELECT * FROM customer WHERE customer_id IN @Ids AND is_deleted =0";
            return dbConnection.Query<Customer>(sql, new { Ids = customerIds }).ToList();
        }
    }
}
