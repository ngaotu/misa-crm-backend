using System;
using System.Collections.Generic;

namespace MISA.CRM.Core.DTOs
{
    /// <summary>
    /// Chuẩn hóa response: { data, meta, error }
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    /// CreatedBy: NTT (15/11/2025)
    public class ApiResponse<T>
    {
        /// <summary>
        /// Dữ liệu trả về (payload)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Thông tin meta (phân trang, tổng số bản ghi, ...)
        /// </summary>
        public object? Meta { get; set; }

        /// <summary>
        /// Thông tin lỗi (nếu có)
        /// </summary>
        public object? Error { get; set; }

        public ApiResponse() { }

        /// <summary>
        /// Thành công, có data, có meta (phân trang)
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="meta">Thông tin meta</param>
        public static ApiResponse<T> Ok(T data, object? meta = null)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Meta = meta,
                Error = null
            };
        }

        /// <summary>
        /// Thành công, tạo mới
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        public static ApiResponse<T> Created(T data)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Meta = null,
                Error = null
            };
        }

        /// <summary>
        /// Cảnh báo (partial success)
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="meta">Thông tin meta</param>
        /// <param name="warning">Thông báo cảnh báo</param>
        public static ApiResponse<T> Warning(T data, object? meta = null, string? warning = null)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Meta = meta,
                Error = warning != null ? new { message = warning } : null
            };
        }

        /// <summary>
        /// Lỗi
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="meta">Thông tin meta</param>
        public static ApiResponse<T> Fail(string message, object? meta = null)
        {
            return new ApiResponse<T>
            {
                Data = default,
                Meta = meta,
                Error = new { message }
            };
        }
    }
}
