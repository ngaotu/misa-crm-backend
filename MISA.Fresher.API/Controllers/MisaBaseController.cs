using Microsoft.AspNetCore.Mvc;
using MISA.CRM.Core.Interfaces.Services;
using MISA.CRM.Core.DTOs;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Reflection;

namespace MISA.CRM.API.Controllers
{
    /// <summary>
    /// Controller cơ sở cung cấp các hành động CRUD cơ bản cho các thực thể.
    /// </summary>
    /// <typeparam name="T">Kiểu thực thể mà controller xử lý.</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    public class MisaBaseController<T> : ControllerBase
    {
        private readonly IBaseService<T> _baseService;

        /// <summary>
        /// Khởi tạo controller cơ sở với service tương ứng được inject.
        /// </summary>
        /// <param name="baseService">Service chứa logic nghiệp vụ cho thực thể T.</param>
        /// CreatedBy: NTT (15/11/2025)
        public MisaBaseController(IBaseService<T> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// CreatedBy: NTT (15/11/2025)
        [HttpGet]
        public virtual IActionResult Get()
        {
            var entities = _baseService.GetAll();
            return Ok(ApiResponse<IEnumerable<T>>.Ok(entities));
        }

        /// <summary>
        /// Lấy bản ghi theo Id
        /// </summary>
        /// <param name="id">Id bản ghi cần lấy</param>
        /// <returns>Bản ghi</returns>
        /// CreatedBy: NTT (15/11/2025)
        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult GetById(Guid id)
        {
            var res = _baseService.GetById(id);
            return Ok(ApiResponse<T>.Ok(res));
        }

        /// <summary>
        /// Thêm mới một bản ghi cho thực thể.
        /// </summary>
        /// <param name="entity">Đối tượng thực thể cần thêm (trong body của request).</param>
        /// <returns>Trả về số bản ghi bị ảnh hưởng (1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        [HttpPost]
        public virtual IActionResult Post([FromBody] T entity)
        {
            var res = _baseService.Insert(entity);
            return StatusCode(201, ApiResponse<int>.Created(res));
        }

        /// <summary>
        /// Cập nhật dữ liệu cho thực thể
        /// </summary>
        /// <param name="entity">Đối tượng thực thể cần thêm (trong body của request).</param>
        /// <param name="id">Id của bản ghi cần lấy</param>
        /// <returns>Trả về số bản ghi bị ảnh hưởng thường là 1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        [HttpPut]
        [Route("{id}")]
        public virtual IActionResult Put(Guid id, [FromBody] T entity)
        {
            var res = _baseService.Update(id, entity);
            return Ok(ApiResponse<int>.Ok(res));
        }

        /// <summary>
        /// Xóa mềm dữ liệu thực thể (đánh dấu is_deleted = True nhưng k xóa trong database)
        /// </summary>
        /// <param name="id">Id của bản ghi cần xóa</param>
        /// <returns>Trả về số bản ghi bị ảnh hưởng thường là 1 nếu thành công.</returns>
        /// CreatedBy: NTT (15/11/2025)
        [HttpDelete]
        [Route("{id}")]
        public virtual IActionResult Delete(Guid id)
        {
            var res = _baseService.Delete(id);
            return Ok(ApiResponse<int>.Ok(res));
        }

        /// <summary>
        /// Tạo mã tự động cho entity theo format: prefix + yyyyMM +6 số tăng dần
        /// Tự động phát hiện từ [AutoGenerateCode] attribute
        /// </summary>
        /// <returns>Mã được sinh tự động</returns>
        /// CreatedBy: NTT (15/11/2025)
        [HttpGet("generate-code")]
        public virtual IActionResult GenerateCode()
        {
            var code = _baseService.GenerateCode();
            return Ok(ApiResponse<string>.Ok(code));
        }
    }
}
