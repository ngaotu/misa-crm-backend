using System;
using System.Collections.Generic;
using MISA.CRM.Core.Entities;
using MISA.CRM.Core.Interfaces.Repositories;
using MISA.CRM.Core.Interfaces.Services;
using MISA.CRM.Core.DTOs.Customer;
using MISA.CRM.Core.DTOs.Common;
using MISA.CRM.Core.Exceptions;
using MISA.CRM.Core.Configuration;
using Microsoft.AspNetCore.Http;
using System.Text;
using CsvHelper;
using System.Globalization;
using System.IO;

namespace MISA.CRM.Core.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho khách hàng
    /// </summary>
    /// Created By: NTT (15/11/2025)
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Khởi tạo CustomerService với repository tương ứng
        /// </summary>
        /// <param name="customerRepository">Repository khách hàng</param>
        /// Created By: NTT (15/11/2025)
        public CustomerService(ICustomerRepository customerRepository) : base(customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang + sắp xếp + tìm kiếm chung
        /// </summary>
        /// <param name="search">Từ khóa tìm kiếm chung</param>
        /// <param name="page">Trang hiện tại</param>
        /// <param name="pageSize">Số bản ghi/trang</param>
        /// <param name="sortBy">Cột sắp xếp</param>
        /// <param name="sortDirection">Hướng sắp xếp</param>
        /// <returns>Kết quả phân trang khách hàng</returns>
        /// Created By: NTT (16/11/2025)
        public PagedResult<Customer> GetPagedCustomers(string? search = null, int page = 1, int pageSize = 10, string? sortBy = null, string? sortDirection = null)
        {
            var pagedResult = _customerRepository.GetPagedCustomers(search, page, pageSize, sortBy, sortDirection);
            return pagedResult;

        }

        /// <summary>
        /// Export danh sách khách hàng được chọn ra CSV (checkbox/bulk action)
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng cần export</param>
        /// <returns>File CSV dưới dạng byte array</returns>
        /// Created By: NTT (16/11/2025)
        public byte[] ExportCustomersToCSV(List<Guid> customerIds)
        {
            var customers = _customerRepository.GetByIds(customerIds);
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, new UTF8Encoding(true));
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            writer.Write('\uFEFF');
            foreach (var header in CustomerCsvMapping.ExportHeaders)
            {
                csv.WriteField(header);
            }
            csv.NextRecord();
            foreach (var customer in customers)
            {
                csv.WriteField(customer.CustomerCode ?? "");
                csv.WriteField(customer.CustomerFullName ?? "");
                csv.WriteField(customer.CustomerEmail ?? "");
                // Thêm dấu phẩy đầu để Excel giữ số0 đầu
                csv.WriteField(!string.IsNullOrEmpty(customer.CustomerPhone) ? $",{customer.CustomerPhone}" : "");
                csv.WriteField(customer.CustomerType ?? "");
                csv.WriteField(customer.CustomerShippingAddr ?? "");
                csv.WriteField(!string.IsNullOrEmpty(customer.CustomerTaxCode) ? $",{customer.CustomerTaxCode}" : "");
                csv.WriteField(customer.CustomerLastPurchaseDate.HasValue && customer.CustomerLastPurchaseDate.Value != DateTime.MinValue
                 ? customer.CustomerLastPurchaseDate.Value.ToString("dd/MM/yyyy")
                 : "");
                csv.WriteField(customer.CustomerPurchasedItems ?? "");
                csv.WriteField(customer.CustomerLastestPurchasedItems ?? "");
                csv.NextRecord();
            }
            writer.Flush();
            return memoryStream.ToArray();
        }


        private string? CleanString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            // Loại bỏ ký tự nháy đơn, phẩy ở đầu và khoảng trắng đầu/cuối
            return value.TrimStart('\'', ',').Trim();
        }

        /// <summary>
        /// Import khách hàng từ file CSV
        /// Áp dụng Partial Success Strategy - bản ghi hợp lệ được lưu, bản ghi lỗi báo chi tiết
        /// </summary>
        /// <param name="csvFile">File CSV upload từ client</param>
        /// <returns>Kết quả import với thống kê và danh sách lỗi chi tiết</returns>
        /// Created By: NTT (16/11/2025)
        public ImportResult ImportCustomersFromCSV(IFormFile csvFile)
        {
            var result = new ImportResult();

            using var stream = csvFile.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Đọc header
            if (!csv.Read()) return result; // empty file
            csv.ReadHeader();

            int rowNumber =1; // header is row1 logically
            while (csv.Read())
            {
                rowNumber++;
                result.TotalRows++;

                // Build a DTO for error reporting
                var dto = new CustomerImportDto
                {
                    RowNumber = rowNumber,
                    FullName = GetFieldSafe(csv, "Họ và tên"),
                    Phone = GetFieldSafe(csv, "Số điện thoại"),
                    Email = GetFieldSafe(csv, "Email"),
                    Address = GetFieldSafe(csv, "Địa chỉ"),
                    CustomerType = GetFieldSafe(csv, "Loại khách hàng"),
                    TaxCode = GetFieldSafe(csv, "Mã số thuế"),
                    LastPurchaseDate = ParseDateSafe(GetFieldSafe(csv, "Ngày mua gần nhất")),
                    PurchasedItems = GetFieldSafe(csv, "Hàng hóa đã mua"),
                    LatestPurchasedItems = GetFieldSafe(csv, "Hàng hóa mua gần nhất")
                };

                try
                {
                    // Validate CustomerType nếu có - nếu null/empty thì bỏ qua
                    if (!CustomerCsvMapping.IsValidCustomerType(dto.CustomerType))
                    {
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = dto.RowNumber,
                            ErrorMessage = $"Loại khách hàng không hợp lệ. Chỉ chấp nhận: {string.Join(", ", CustomerCsvMapping.AllowedCustomerTypes)} hoặc để trống",
                            OriginalRow = dto
                        });
                        result.ErrorCount++;
                        continue; // Skip this row
                    }

                    // Làm sạch phone và taxcode trước khi lưu
                    var customer = new Customer
                    {
                        CustomerId = Guid.NewGuid(),
                        // KHÔNG gán CustomerCode từ file, để hệ thống tự sinh
                        CustomerFullName = string.IsNullOrWhiteSpace(dto.FullName) ? null : dto.FullName.Trim(),
                        CustomerPhone = CleanString(dto.Phone),
                        CustomerEmail = CleanString(dto.Email),
                        CustomerShippingAddr = CleanString(dto.Address),
                        CustomerType = CleanString(dto.CustomerType),
                        CustomerTaxCode = CleanString(dto.TaxCode),
                        CustomerLastPurchaseDate = dto.LastPurchaseDate ?? DateTime.MinValue,
                        CustomerPurchasedItems = CleanString(dto.PurchasedItems),
                        CustomerLastestPurchasedItems = CleanString(dto.LatestPurchasedItems),
                        IsDeleted = false
                    };

                    // Insert immediately to avoid keeping many items in memory
                    Insert(customer);
                    result.SuccessCount++;
                }
                catch (ValidateException ex)
                {
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = dto.RowNumber,
                        ErrorMessage = ex.Message,
                        OriginalRow = dto
                    });
                    result.ErrorCount++;
                }
                catch (ConflictException ex)
                {
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = dto.RowNumber,
                        ErrorMessage = ex.Message,
                        OriginalRow = dto
                    });
                    result.ErrorCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = dto.RowNumber,
                        ErrorMessage = $"Lỗi hệ thống: {ex.Message}",
                        OriginalRow = dto
                    });
                    result.ErrorCount++;
                }
            }

            return result;
        }

        /// <summary>
        /// Lấy giá trị field an toàn (không throw exception nếu không tìm thấy)
        /// </summary>
        /// <param name="csv">CsvReader</param>
        /// <param name="fieldName">Tên cột</param>
        /// <returns>Giá trị field hoặc null</returns>
        /// Created By: NTT (16/11/2025)
        private string? GetFieldSafe(CsvReader csv, string fieldName)
        {
            try
            {
                return csv.GetField(fieldName);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse ngày tháng an toàn từ string
        /// </summary>
        /// <param name="dateString">Chuỗi ngày tháng</param>
        /// <returns>Giá trị DateTime hoặc null nếu không hợp lệ</returns>
        /// Created By: NTT (16/11/2025)
        private DateTime? ParseDateSafe(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            try
            {
                // Hỗ trợ các format phổ biến
                if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date1))
                    return date1;

                if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var date2))
                    return date2;

                if (DateTime.TryParse(dateString, out var date3))
                    return date3;

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gán loại khách hàng cho nhiều khách hàng theo danh sách ID.
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng</param>
        /// <param name="customerType">Loại khách hàng cần gán</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// Created By: NTT (15/11/2025)
        public int AssignCustomerType(List<Guid> customerIds, string customerType)
        {
            if (customerIds == null || customerIds.Count ==0)
                return 0;
            var res =  _customerRepository.AssignCustomerType(customerIds, customerType);
            if (res == 0) {
                throw new NotFoundException("Không tìm thấy dữ liệu");
            }
            return res;
        }

        /// <summary>
        /// Xóa mềm nhiều khách hàng theo danh sách ID.
        /// </summary>
        /// <param name="customerIds">Danh sách ID khách hàng cần xóa</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// Created By: NTT (15/11/2025)
        public int BulkDelete(List<Guid> customerIds)
        {
            if (customerIds == null || customerIds.Count ==0)
                return 0;
            var res = _customerRepository.BulkDelete(customerIds);
            if (res == 0)
            {
                throw new NotFoundException("Không tìm thấy dữ liệu");
            }
            return res;
        }

        /// <summary>
        /// Kiểm tra tồn tại khách hàng theo Email
        /// </summary>
        /// <param name="email">Địa chỉ Email</param>
        /// <returns>true nếu tồn tại, ngược lại false</returns>
        /// Created By: NTT (15/11/2025)
        public bool ExistsByEmail(string email, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return _customerRepository.ExistsByProperty(nameof(Customer.CustomerEmail), email.Trim(), excludeId);
        }

        /// <summary>
        /// Kiểm tra tồn tại khách hàng theo Số điện thoại
        /// </summary>
        /// <param name="phone">Số điện thoại</param>
        /// <returns>true nếu tồn tại, ngược lại false</returns>
        /// Created By: NTT (15/11/2025)
        public bool ExistsByPhone(string phone, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return _customerRepository.ExistsByProperty(nameof(Customer.CustomerPhone), phone.Trim(), excludeId);
        }
    }
}
