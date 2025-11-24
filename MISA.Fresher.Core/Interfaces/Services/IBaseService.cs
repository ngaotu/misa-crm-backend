using System;
using System.Collections.Generic;

namespace MISA.CRM.Core.Interfaces.Services
{
    /// <summary>
    /// Interface cho nghiệp vụ cơ bản
    /// </summary>
    /// Created By: NTT (15/11/2025)
    public interface IBaseService<T>
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
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        int Insert(T entity);

        /// <summary>
        /// Cập nhật bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần cập nhật.</param>
        /// <param name="entity">Đối tượng thực thể chứa dữ liệu cập nhật.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là1 nếu cập nhật thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        int Update(Guid entityId, T entity);

        /// <summary>
        /// Xóa bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần xóa.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        int Delete(Guid entityId);

        /// <summary>
        /// Check dữ liệu đã trùng lặp chưa (theo mã, email, số điện thoại...) 
        /// </summary>
        /// <param name="entity">Đối tượng cần check</param>
        /// <param name="excludeId">Id cần loại trừ (dùng khi update)</param>
        void CheckDuplicate(T entity, Guid? excludeId);

        /// <summary>
        /// Sinh mã tự động theo format: prefix + yyyyMM + 6 số tăng dần
        /// Tự động phát hiện property có [AutoGenerateCode("prefix")] attribute
        /// Ví dụ: KH202411000123
        /// </summary>
        /// <returns>Mã được sinh tự động</returns>
        /// CreatedBy: NTT (15/11/2025)
        string GenerateCode();
    }
}
