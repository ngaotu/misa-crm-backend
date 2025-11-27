using Microsoft.AspNetCore.Mvc;
using MISA.CRM.API.Controllers;
using MISA.CRM.Core.Entities;
using MISA.CRM.Core.Interfaces.Services;
using MISA.CRM.Core.DTOs;
using MISA.CRM.Core.DTOs.Common;
using MISA.CRM.Core.DTOs.Customer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace MISA.CRM.API.Controllers
{
    /// <summary>
    /// Controller xử lý các yêu cầu liên quan đến khách hàng (Customers).
    /// Sử dụng trực tiếp entity Customer kế thừa từ MisaBaseController.
    /// </summary>
    /// CreatedBy: NTT (15/11/2025)
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : MisaBaseController<Customer>
    {
        private readonly ICustomerService _customerService;
        private readonly IFileStorageService? _fileStorage;

        public CustomersController(ICustomerService customerService, IFileStorageService? fileStorage = null) : base(customerService)
        {
            _customerService = customerService;
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// API Lấy danh sách khách hàng (Có phân trang + sắp xếp + tìm kiếm theo tên, email, sdt)
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm chung</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi/trang</param>
        /// <param name="sortBy">Cột sắp xếp</param>
        /// <param name="sortDirection">Hướng sắp xếp</param>
        /// <returns>Kết quả phân trang khách hàng</returns>
        /// CreatedBy: NTT (16/11/2025)
        [HttpGet]
        [Route("paging")]
        public IActionResult Get([FromQuery] string? search = null, [FromQuery] int page =1, [FromQuery] int pageSize =10, [FromQuery] string? sortBy = null, [FromQuery] string? sortDirection = null)
        {
            var result = _customerService.GetPagedCustomers(search, page, pageSize, sortBy, sortDirection);
            var meta = new {
                page = result.CurrentPage,
                pageSize = result.PageSize,
                total = result.TotalRecords
            };
            return Ok(ApiResponse<List<Customer>>.Ok(result.Items, meta));
        }

        /// <summary>
        /// API tạo khách hàng mới, hỗ trợ upload avatar (multipart/form-data)
        /// </summary>
        /// <param name="customer">Thông tin khách hàng</param>
        /// <param name="avatar">File chứa ảnh khách hàng</param>
        /// CreatedBy: NTT (16/11/2025)
        [HttpPost("with-file")]
        [Consumes("multipart/form-data")]
        public IActionResult PostWithFile([FromForm] Customer customer, IFormFile? avatar)
        {
            if (avatar != null && _fileStorage is not null)
            {
                var savedPath = _fileStorage.SaveFile(avatar, "uploads/avatars");
                customer.CustomerAvatar = savedPath;
            }

            var res = _customerService.Insert(customer);
            return StatusCode(201, ApiResponse<int>.Created(res));
        }

        /// <summary>
        /// API Update customer, hỗ trợ upload avatar (multipart/form-data)
        /// </summary>
        /// <param name="id">ID khách hàng cần cập nhật</param>
        /// <param name="customer">Thông tin khách hàng</param>
        /// <param name="avatar">File chứa ảnh khách hàng</param>
        /// CreatedBy: NTT (16/11/2025)
        [HttpPut("{id}/with-file")]
        [Consumes("multipart/form-data")]
        public IActionResult PutWithFile(Guid id, [FromForm] Customer customer, IFormFile? avatar)
        {
            var existing = _customerService.GetById(id);

            if (avatar != null && _fileStorage is not null)
            {
                var newPath = _fileStorage.SaveFile(avatar, "uploads/avatars");
                // delete old if exists and different
                if (!string.IsNullOrWhiteSpace(existing.CustomerAvatar) && existing.CustomerAvatar != newPath)
                {
                    _fileStorage.DeleteFile(existing.CustomerAvatar);
                }
                customer.CustomerAvatar = newPath;
            }
            else
            {
                customer.CustomerAvatar = existing.CustomerAvatar;
            }

            var res = _customerService.Update(id, customer);
            return Ok(ApiResponse<int>.Ok(res));
        }

        /// <summary>
        /// API Export danh sách khách hàng được chọn ra CSV (checkbox/bulk action)
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng cần export</param>
        /// <returns>File CSV để download</returns>
        /// CreatedBy: NTT (16/11/2025)
        [HttpPost("export")]
        public IActionResult ExportCSV([FromBody] List<Guid> customerIds)
        {
            var csvData = _customerService.ExportCustomersToCSV(customerIds);
            var fileName = $"MISA_CRM_Customers_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(csvData, "text/csv; charset=utf-8", fileName);
        }

        /// <summary>
        /// API Import khách hàng từ file CSV
        /// Áp dụng Partial Success Strategy - bản ghi hợp lệ được lưu, bản ghi lỗi báo chi tiết
        /// </summary>
        /// <param name="file">File CSV upload</param>
        /// <returns>Kết quả import với thống kê và danh sách lỗi chi tiết</returns>
        /// CreatedBy: NTT (16/11/2025)
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public IActionResult ImportCSV(IFormFile file)
        {
            // Validate file input
            if (file == null || file.Length ==0)
            {
                return BadRequest(ApiResponse<object>.Fail("Vui lòng chọn file CSV"));
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail("Chỉ chấp nhận file CSV"));
            }

            if (file.Length >10 *1024 *1024)
            {
                return BadRequest(ApiResponse<object>.Fail("Kích thước file không được vượt quá10MB"));
            }

            var result = _customerService.ImportCustomersFromCSV(file);

            if (result.ErrorCount >0)
            {
                return Ok(ApiResponse<ImportResult>.Warning(result, null, $"Import hoàn tất với {result.ErrorCount} lỗi. {result.Summary}"));
            }

            return Ok(ApiResponse<ImportResult>.Ok(result));
        }

        /// <summary>
        /// API Gán loại khách hàng cho nhiều khách hàng theo danh sách ID (bulk action).
        /// </summary>
        /// <param name="request">Danh sách ID và loại khách hàng cần gán</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: NTT (16/11/2025)
        [HttpPost("bulk-assign-type")]
        public IActionResult BulkAssignType([FromBody] BulkAssignTypeRequest request)
        {
            var affected = _customerService.AssignCustomerType(request.CustomerIds, request.CustomerType);
            return Ok(ApiResponse<int>.Ok(affected));
        }

        /// <summary>
        /// Xóa mềm nhiều khách hàng theo danh sách ID (bulk action).
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// CreatedBy: NTT (16/11/2025)
        [HttpPost("bulk-delete")]
        public IActionResult BulkDelete([FromBody] List<Guid> customerIds)
        {
            var affected = _customerService.BulkDelete(customerIds);
            return Ok(ApiResponse<int>.Ok(affected));
        }

        /// <summary>
        /// Kiểm tra email trùng
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="excludeId">ID của khách hàng</param>
        /// <returns>True nếu email đã tồn tại, ngược lại false</returns>
        /// CreatedBy: NTT (18/11/2025)
        [HttpGet("check-duplicate/email")]
        public IActionResult CheckDuplicateEmail([FromQuery] string email, [FromQuery] Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(ApiResponse<object>.Fail("Vui lòng cung cấp email để kiểm tra"));

            var exists = _customerService.ExistsByEmail(email, excludeId);
            return Ok(ApiResponse<bool>.Ok(exists));
        }

        /// <summary>
        /// Kiểm tra phone trùng
        /// </summary>
        /// <param name="phone">Số điện thoại cần kiểm tra</param>
        /// <param name="excludeId">ID của khách hàng</param>
        /// <returns>True nếu số điện thoại đã tồn tại, ngược lại false</returns>
        /// CreatedBy: NTT (18/11/2025)        
        [HttpGet("check-duplicate/phone")]
        public IActionResult CheckDuplicatePhone([FromQuery] string phone, [FromQuery] Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest(ApiResponse<object>.Fail("Vui lòng cung cấp số điện thoại để kiểm tra"));

            var exists = _customerService.ExistsByPhone(phone, excludeId);
            return Ok(ApiResponse<bool>.Ok(exists));
        }
    }
}
