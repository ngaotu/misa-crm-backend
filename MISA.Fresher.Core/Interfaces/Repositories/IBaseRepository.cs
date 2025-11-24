using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface đại diện cho các phương thức thao tác cơ bản với thực thể trong kho dữ liệu.
    /// </summary>
    /// <param name="T">Kiểu của thực thể.</param>
    /// CreatedBy: NTT (15/11/2025)
    public interface IBaseRepository<T> : IDisposable
    {
        /// <summary>
        /// Lấy tất cả bản ghi của thực thể.
        /// </summary>
        /// <returns>Danh sách các bản ghi của thực thể.</returns>
        /// CreatedBy: NTT (15/11/2025)
        List<T> GetAll();

        /// <summary>
        /// Lấy bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần lấy.</param>
        /// <returns>Bản ghi tương ứng với Id; nếu không tồn tại trả về null.</returns>
        /// CreatedBy: NTT (15/11/2025)
        T GetById(Guid entityId);

        /// <summary>
        /// Thêm một bản ghi mới cho thực thể.
        /// </summary>
        /// <param name="entity">Đối tượng thực thể cần thêm.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        int Insert(T entity);

        /// <summary>
        /// Cập nhật bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần cập nhật.</param>
        /// <param name="entity">Đối tượng thực thể chứa dữ liệu cập nhật.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là 1 nếu cập nhật thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        int Update(Guid entityId, T entity);

        /// <summary>
        /// Xóa 1 bản ghi cho thực thể.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần xóa.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        int Delete(Guid entityId);

        /// <summary>
        /// Kiểm tra xem có bản ghi nào tồn tại với giá trị của property đã cho hay không.
        /// </summary>
        /// <param name="propertyName">Tên property trong class</param>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <param name="excludeId">Nếu cung cấp, sẽ loại trừ bản ghi có khóa chính này (dùng cho update).</param>
        /// <returns>True nếu tồn tại bản ghi trùng, false nếu không.</returns>
        /// CreatedBy: NTT (15/11/2025)
        bool ExistsByProperty(string propertyName, object value, Guid? excludeId = null);

        /// <summary>
        /// Lấy số thứ tự tiếp theo để sinh mã tự động (KH + yyyyMM + 6 số tăng dần)
        /// </summary>
        /// <param name="codeColumn">Tên cột chứa mã (ví dụ: customer_code)</param>
        /// <param name="prefix">Tiền tố (ví dụ: KH)</param>
        /// <param name="yearMonth">Năm tháng (ví dụ: 202411)</param>
        /// <returns>Số thứ tự tiếp theo (ví dụ: 000123)</returns>
        /// CreatedBy: NTT (15/11/2025)
        int GetNextSequenceNumber(string codeColumn, string prefix, string yearMonth);

        /// <summary>
        /// Sinh mã tự động theo format: prefix + yyyyMM + 6 số tăng dần
        /// Tự động phát hiện property có [AutoGenerateCode("prefix")] attribute
        /// </summary>
        /// <returns>Mã được sinh tự động</returns>
        /// CreatedBy: NTT (15/11/2025)
        string GenerateCode();
    }
}
