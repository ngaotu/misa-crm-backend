using System.Text.Json;
using MISA.CRM.Core.Exceptions;

namespace MISA.CRM.API.Middlewares
{
    /// <summary>
    /// Middleware xử lý ngoại lệ chung cho toàn bộ ứng dụng.
    /// Bắt và chuẩn hoá các exception vào response JSON theo cấu trúc ApiResponse kiểu chung.
    /// </summary>
    /// <remarks>CreatedBy: NTT (18/11/2025)</remarks>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Khởi tạo middleware
        /// </summary>
        /// <param name="next">Delegate tiếp theo trong pipeline</param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Thực thi middleware: gọi pipeline tiếp theo và bắt các exception để trả về response chuẩn.
        /// </summary>
        /// <param name="context">HttpContext của request hiện tại</param>
        /// <returns>Task hoàn thành khi xử lý xong</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteUnifiedErrorAsync(context,404, "NotFound", ex.Message);
            }
            catch (ValidateException ex)
            {
                await WriteUnifiedErrorAsync(context,400, "Validation", ex.Message);
            }
            catch (ConflictException ex)
            {
                await WriteUnifiedErrorAsync(context,409, "Conflict", ex.Message);
            }
            catch (Exception)
            {
                await WriteUnifiedErrorAsync(context,500, "ServerError", "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Ghi response lỗi dưới dạng JSON chuẩn của API.
        /// </summary>
        /// <param name="context">HttpContext hiện tại</param>
        /// <param name="statusCode">Mã HTTP trả về</param>
        /// <param name="type">Kiểu lỗi (ví dụ: Validation, NotFound, Conflict, ServerError)</param>
        /// <param name="message">Thông điệp lỗi hiển thị</param>
        /// <param name="details">Chi tiết lỗi (tùy chọn)</param>
        /// <returns>Task hoàn thành khi đã viết response</returns>
        /// CreatedBy: NTT (18/11/2025)
        private static async Task WriteUnifiedErrorAsync(HttpContext context, int statusCode, string type, string message, object? details = null)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new
            {
                data = (object?)null,
                meta = (object?)null,
                error = new
                {
                    code = statusCode,
                    type,
                    message,
                    details
                }
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(payload, options);
            await context.Response.WriteAsync(json);
        }
    }
}
