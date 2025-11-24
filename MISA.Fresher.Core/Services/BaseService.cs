using MISA.CRM.Core.Exceptions;
using MISA.CRM.Core.Interfaces.Repositories;
using MISA.CRM.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MISA.CRM.Core.MISAAtributes;

namespace MISA.CRM.Core.Services
{
    /// <summary>
    /// Lớp service cơ sở cung cấp các logic nghiệp vụ chung cho các thực thể.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể mà service xử lý.</typeparam>
    /// <remarks>CreatedBy: NTT</remarks>
    /// Created By: NTT (15/11/2025)
    public class BaseService<T> : IBaseService<T>
    {
        private readonly IBaseRepository<T> _baseRepository;

        /// <summary>
        /// Khởi tạo BaseService với repository tương ứng.
        /// </summary>
        /// <param name="baseRepository">Repository để truy xuất dữ liệu cho thực thể T.</param>
        public BaseService(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Sinh mã tự động - gọi repository xử lý logic
        /// </summary>
        public virtual string GenerateCode()
        {

           return _baseRepository.GenerateCode();
           
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu của thực thể trước khi lưu.
        /// Nếu có trường bắt buộc bị thiếu sẽ ném ra ngoại lệ <see cref="ValidateException"/>.
        /// </summary>
        /// <param name="entity">Đối tượng thực thể cần validate.</param>
        /// <returns>True nếu dữ liệu hợp lệ.</returns>
        /// <exception cref="ValidateException">Khi dữ liệu không hợp lệ.</exception>
        public bool ValidateData(T entity)
        {
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);
                var stringValue = value as string;
                // 1. Kiểm tra NotEmpty
                var notEmptyAttr = prop.GetCustomAttribute<NotEmptyAttribute>();
                if (notEmptyAttr != null)
                {
                    if (value == null || (stringValue != null && string.IsNullOrWhiteSpace(stringValue)))
                    {
                        var message = notEmptyAttr.Message ?? $"{prop.Name} không được để trống";
                        throw new ValidateException(message);
                    }
                }

                // Chỉ validate các attribute khác nếu có giá trị
                if (stringValue != null && !string.IsNullOrWhiteSpace(stringValue))
                {
                    // 2. Kiểm tra Email Format
                    var emailFormatAttr = prop.GetCustomAttribute<EmailFormatAttribute>();
                    if (emailFormatAttr != null)
                    {
                        if (!IsValidEmail(stringValue))
                        {
                            throw new ValidateException(emailFormatAttr.Message ?? "Email không đúng định dạng");
                        }
                    }

                    // 3. Kiểm tra Phone Length (10-11 số)
                    var phoneLengthAttr = prop.GetCustomAttribute<PhoneLengthAttribute>();
                    if (phoneLengthAttr != null)
                    {
                        if (!IsValidPhoneLength(stringValue))
                        {
                            throw new ValidateException(phoneLengthAttr.Message ?? "Số điện thoại phải có 10-11 chữ số");
                        }
                    }

                    // 4. Kiểm tra MaxLength
                    var maxLengthAttr = prop.GetCustomAttribute<MaxLengthCustomAttribute>();
                    if (maxLengthAttr != null)
                    {
                        if (stringValue.Length > maxLengthAttr.MaxLength)
                        {
                            throw new ValidateException(maxLengthAttr.Message ?? $"{prop.Name} không được vượt quá {maxLengthAttr.MaxLength} ký tự");
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Kiểm tra trùng dữ liệu dựa trên các thuộc tính được gắn [Unique].
        /// Nếu đang update có thể truyền excludeId để loại trừ bản ghi hiện tại.
        /// Ném ConflictException khi phát hiện trùng.
        /// </summary>
        /// <param name="entity">Đối tượng cần kiểm tra.</param>
        /// <param name="excludeId">Id cần loại trừ (dùng khi update).</param>
        /// CreatedBy: NTT (15/11/2025)
        public void CheckDuplicate(T entity, Guid? excludeId = null)
        {
            var props = typeof(T).GetProperties();

            // Nếu không có excludeId, cố gắng lấy từ entity  
            var pkProp = props.FirstOrDefault(p => p.GetCustomAttribute<PrimaryKey>() != null);
            if (excludeId == null && pkProp != null)
            {
                var pkValue = pkProp.GetValue(entity);
                if (pkValue is Guid g && g != Guid.Empty)
                    excludeId = g;
            }

            foreach (var prop in props)
            {
                var uniqueAttr = prop.GetCustomAttribute<UniqueAttribute>();
                if (uniqueAttr == null) continue;

                var value = prop.GetValue(entity);
                if (value == null) continue;

                if (_baseRepository.ExistsByProperty(prop.Name, value, excludeId))
                {
                    var message = string.IsNullOrWhiteSpace(uniqueAttr.Message)
         ? $"{prop.Name} bị trùng."
                    : uniqueAttr.Message;
                    throw new ConflictException(message);
                }
            }
        }

        /// <summary>
        /// Lấy tất cả bản ghi của entity T.
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi của entity T.</returns>
        /// CreatedBy: NTT (15/11/2025)
        public List<T> GetAll()
        {
            return _baseRepository.GetAll();
        }

        /// <summary>
        /// Lấy bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id bản ghi cần lấy.</param>
        /// <returns>Bản ghi tương ứng.</returns>
        /// <exception cref="NotFoundException">Khi không tìm thấy bản ghi với Id cung cấp.</exception>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public T GetById(Guid entityId)
        {
            var entity = _baseRepository.GetById(entityId);
            if (entity == null)
            {
                throw new NotFoundException($"Entity with Id {entityId} not found.");
            }
            return entity;
        }

        /// <summary>
        /// Thêm một bản ghi mới cho thực thể.
        /// </summary>
        /// <param name="entity">Đối tượng thực thể cần thêm.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là 1 nếu thành công).</returns>
        /// <exception cref="ValidateException">Khi dữ liệu không hợp lệ.</exception>
        /// CreatedBy: NTT (15/11/2025)
        public virtual int Insert(T entity)
        {
            ValidateData(entity);
            CheckDuplicate(entity, null);
            return _baseRepository.Insert(entity);
        }

        /// <summary>
        /// Cập nhật bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần cập nhật.</param>
        /// <param name="entity">Đối tượng thực thể chứa dữ liệu cập nhật.</param>
        /// <returns>Số lượng bản ghi bị ���nh hưởng (thường là 1 nếu cập nhật thành công).</returns>
        /// <exception cref="ValidateException">Khi dữ liệu không hợp lệ.</exception>
        /// <exception cref="NotFoundException">Khi dữ liệu không tồn tại</exception>
        /// CreatedBy: NTT (15/11/2025)
        public virtual int Update(Guid entityId, T entity)
        {
            ValidateData(entity);
            CheckDuplicate(entity, entityId);
            var res = _baseRepository.Update(entityId, entity);
            if (res == 0)
            {
                throw new NotFoundException("Không tìm thấy dữ liệu");
            }
            return res;
        }

        /// <summary>
        /// Xóa bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi bị xóa.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là 1 nếu xóa thành công).</returns>
        /// <exception cref="NotFoundException">Nếu không tìm thấy bản ghi cần xóa</exception>
        /// CreatedBy: NTT (15/11/2025)
        public virtual int Delete(Guid entityId)
        {
            var res = _baseRepository.Delete(entityId);
            if (res == 0)
            {
                throw new NotFoundException("Không tìm thấy dữ liệu");
            }
            return res;
        }

        /// <summary>
        /// Kiểm tra định dạng email hợp lệ
        /// </summary>
        /// CreatedBy: NTT (15/11/2025)
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại có 10-11 chữ số
        /// </summary>
        /// CreatedBy: NTT (15/11/2025)
        private bool IsValidPhoneLength(string phone)
        {
            // Loại bỏ khoảng trắng và ký tự đặc biệt
            var cleanPhone = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");

            // Kiểm tra độ dài 10-11 số và chỉ chứa số
            return cleanPhone.Length >= 10 && cleanPhone.Length <= 11 && cleanPhone.All(char.IsDigit);
        }
    }
}
