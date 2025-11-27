using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.CRM.Core.Interfaces.Repositories;
using MISA.CRM.Core.MISAAtributes;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.CRM.Infrastructure.Repositories
{
    /// <summary>
    /// Lớp cơ sở chứa các thao tác chung cho các repository cụ thể.
    /// Phụ trách tương tác trực tiếp với cơ sở dữ liệu.
    /// </summary>
    /// <typeparam name="T">Kiểu của thực thể mà repository thao tác.</typeparam>
    /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
    public class BaseRepository<T> : IBaseRepository<T>
    {
        protected readonly string _connectionString;
        protected readonly IDbConnection dbConnection;

        public BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            dbConnection = new MySqlConnection(_connectionString);
        }

        /// <summary>
        /// Lấy tất cả bản ghi của thực thể (chỉ lấy bản ghi chưa bị xóa mềm).
        /// </summary>
        /// <returns>Danh sách các bản ghi của thực thể.</returns>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public List<T> GetAll()
        {
            var tableName = GetTableName();
            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
            string sql;
            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
            {
                sql = $"SELECT * FROM {tableName} WHERE is_deleted =0";
            }
            else
            {
                sql = $"SELECT * FROM {tableName}";
            }
            var entities = dbConnection.Query<T>(sql).ToList();
            return entities;

        }

        /// <summary>
        /// Thêm một bản ghi mới cho thực thể. Nếu có property [AutoGenerateCode] thì tự động sinh mã bằng GenerateCode().
        /// </summary>
        /// <param name="entity">Đối tượng thực thể cần thêm.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là1 nếu thành công).</returns>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public int Insert(T entity)
        {
            // Tự động sinh mã nếu có property [AutoGenerateCode]
            var codeProp = entity.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttribute<AutoGenerateCodeAttribute>() != null);
            if (codeProp != null)
            {
                var codeValue = codeProp.GetValue(entity) as string;
                if (string.IsNullOrWhiteSpace(codeValue))
                {
                    // Gọi hàm GenerateCode của repository
                    var newCode = GenerateCode();
                    codeProp.SetValue(entity, newCode);
                }
            }

            var tableName = GetTableName();
            var props = entity.GetType().GetProperties();
            var columnList = new List<string>();
            var paramList = new List<string>();
            var parameters = new DynamicParameters();
            foreach (var prop in props)
            {
                var primaryKeyAttr = prop.GetCustomAttribute<PrimaryKey>();
                if (primaryKeyAttr != null)
                {
                    prop.SetValue(entity, Guid.NewGuid());
                }

                // Ưu tiên dùng ColumnAttribute nếu có
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                string columnName;
                if (colAttr != null && !string.IsNullOrWhiteSpace(colAttr.Name))
                {
                    columnName = colAttr.Name;
                }
                else
                {

                    columnName = ToSnakeCase(prop.Name);
                }
                columnList.Add(columnName);
                paramList.Add($"@{prop.Name}");
                parameters.Add($"@{prop.Name}", prop.GetValue(entity));
            }

            var sql = $"INSERT INTO {tableName} ({string.Join(",", columnList)}) VALUES ({string.Join(",", paramList)})";
            var res = dbConnection.Execute(sql, parameters);
            return res;
        }

        /// <summary>
        /// Lấy bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần lấy.</param>
        /// <returns>Bản ghi tương ứng với Id; nếu không tồn tại trả về null.</returns>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public T GetById(Guid entityId)
        {
            var tableName = GetTableName();
            var columnName = GetPrimaryKeyColumn();
            Console.WriteLine(columnName);
            string sql = @$"SELECT * FROM {tableName} WHERE {columnName} = @Id";
            var parameters = new DynamicParameters();
            parameters.Add($"@Id", entityId);
            var res = dbConnection.QueryFirstOrDefault<T>(sql, parameters);
            return res;
        }

        /// <summary>
        /// Cập nhật bản ghi theo Id.
        /// </summary>
        /// <param name="entityId">Id của bản ghi cần cập nhật.</param>
        /// <param name="entity">Đối tượng thực thể chứa dữ liệu cập nhật.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (thường là1 nếu cập nhật thành công).</returns>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public int Update(Guid entityId, T entity)
        {
            var tableName = GetTableName();
            var props = entity.GetType().GetProperties();

            // Tìm primary key property
            var pkProp = props.FirstOrDefault(p => p.GetCustomAttribute<PrimaryKey>() != null);

            var setClauses = new List<string>();
            var parameters = new DynamicParameters();

            foreach (var prop in props)
            {
                // Bỏ qua khóa chính
                if (prop.Name.Equals(pkProp.Name, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                string columnName = colAttr != null && !string.IsNullOrWhiteSpace(colAttr.Name)
                                    ? colAttr.Name
                                    : ToSnakeCase(prop.Name);

                setClauses.Add($"{columnName} = @{prop.Name}");
                parameters.Add($"@{prop.Name}", prop.GetValue(entity));
            }

            // Thêm param Id cho where
            parameters.Add("@Id", entityId);

            var sql = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE {GetPrimaryKeyColumn()} = @Id";

            var res = dbConnection.Execute(sql, parameters);
            return res;
        }
        /// <summary>
        /// Xóa mềm bản ghi theo Id (set IsDeleted = true).
        /// </summary>
        /// <param name="entityId">Id của bản ghi bị xóa.</param>
        /// <returns>Số lượng bản ghi bị ảnh hưởng (1 nếu thành công).</returns>
        /// CreatedBy: NTT (15/11/2025)
        public int Delete(Guid entityId)
        {
            var entity = GetById(entityId);
            if (entity == null)
                return 0;
            var isDeletedProp = typeof(T).GetProperty("IsDeleted");
            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
            {
                isDeletedProp.SetValue(entity, true);
                return Update(entityId, entity);
            }
            // Nếu không có IsDeleted thì xóa cứng
            var tableName = GetTableName();
            var columnName = GetPrimaryKeyColumn();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", entityId);
            var sql = $"DELETE FROM {tableName} WHERE {columnName} = @Id";
            var res = dbConnection.Execute(sql, parameters);
            return res;
        }

        /// <summary>
        ///  Lấy tên bảng
        /// </summary>
        /// <returns>Tên bảng</returns>
        /// CreatedBy: NTT (15/11/2025)
        protected string GetTableName()
        {
            // Nếu entity type có attribute TableNameAttribute thì dùng tên trong attribute
            var tableAttr = typeof(T).GetCustomAttribute<TableNameAttribute>();
            if (tableAttr != null && !string.IsNullOrWhiteSpace(tableAttr.Name))
            {
                return tableAttr.Name;
            }

            return typeof(T).Name.ToLower();
        }
        /// <summary>
        /// Chuyển đổi chuỗi từ CamelCase sang snake_case
        /// </summary>
        /// <param name="str">Chuỗi snake_case</param>
        /// <returns></returns>
        /// CreatedBy: NTT (15/11/2025)
        protected string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return string.Concat(
                str.Select((character, index) =>
                index > 0 && char.IsUpper(character)
                ? "_" + char.ToLower(character)
                : char.ToLower(character).ToString()
                )
            );
        }

        public void Dispose()
        {
            dbConnection?.Dispose();
        }

        /// <summary>
        /// Lấy tên cột khóa chính cho thực thể T.
        /// </summary>
        /// <returns>Tên cột khóa chính trong database</returns>
        /// CreatedBy: NTT (15/11/2025)
        protected string GetPrimaryKeyColumn()
        {
            var props = typeof(T).GetProperties();

            // Tìm thuộc tính được gắn PrimaryKey attribute
            var pkProp = props.FirstOrDefault(p => p.GetCustomAttribute<PrimaryKey>() != null);

            // Nếu không tìm thấy, fallback sang thuộc tính có tên kết thúc bằng "Id"
            if (pkProp == null)
            {
                pkProp = props.FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase));
            }
            // Nếu có ColumnAttribute, dùng tên cột trong attribute
            var colAttr = pkProp.GetCustomAttribute<ColumnAttribute>();
            if (colAttr != null && !string.IsNullOrWhiteSpace(colAttr.Name))
            {
                return colAttr.Name;
            }

            return ToSnakeCase(pkProp.Name);
        }

        /// <summary>
        /// Kiểm tra tồn tại bản ghi theo giá trị của property
        /// </summary>
        /// <param name="propertyName">Tên property trong class (ví dụ: "EmployeeEmail").</param>
        /// <param name="value">Giá trị cần kiểm tra.</param>
        /// <param name="excludeId">Nếu cung cấp, sẽ loại trừ bản ghi có khóa chính này (dùng cho update).</param>
        /// <returns>True nếu tồn tại bản ghi trùng, false nếu không.</returns>
        /// CreatedBy: NTT (15/11/2025)
        public bool ExistsByProperty(string propertyName, object value, Guid? excludeId = null)
        {
            var prop = typeof(T).GetProperty(propertyName);
            if (prop == null) return false;

            var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
            string columnName = colAttr != null && !string.IsNullOrWhiteSpace(colAttr.Name)
                                ? colAttr.Name
                                : ToSnakeCase(prop.Name);

            var tableName = GetTableName();
            var pkColumn = GetPrimaryKeyColumn();

            var sql = $"SELECT COUNT(1) FROM {tableName} WHERE {columnName} = @value";
            if (excludeId.HasValue)
            {
                sql += $" AND {pkColumn} <> @excludeId";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@value", value);
            parameters.Add("@excludeId", excludeId);

            var count = dbConnection.QuerySingle<int>(sql, parameters);
            return count > 0;
        }

        /// <summary>
        /// Sinh mã tự động theo format: prefix + yyyyMM + 6 số tăng dần
        /// Tự động phát hiện property có [AutoGenerateCode("prefix")] attribute
        /// </summary>
        /// <returns>Mã được sinh tự động</returns>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public virtual string GenerateCode()
        {
            // Tìm property có AutoGenerateCode attribute
            var codeProperty = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttribute<AutoGenerateCodeAttribute>() != null);

            var autoCodeAttr = codeProperty.GetCustomAttribute<AutoGenerateCodeAttribute>();
            var prefix = autoCodeAttr.Prefix;

            var columnAttr = codeProperty.GetCustomAttribute<ColumnAttribute>();
            var codeColumn = columnAttr?.Name ?? ToSnakeCase(codeProperty.Name);

            var now = DateTime.Now;
            var yearMonth = now.ToString("yyyyMM");

            var nextNumber = GetNextSequenceNumber(codeColumn, prefix, yearMonth);

            // Format số thành 6 chữ số (000001, 000002, ...)
            var sequenceNumber = nextNumber.ToString("D6");

            return $"{prefix}{yearMonth}{sequenceNumber}";
        }

        /// <summary>
        /// Lấy số thứ tự tiếp theo để sinh mã tự động theo format: prefix + yyyyMM + 6 số tăng dần
        /// Ví dụ: KH202411000123
        /// </summary>
        /// <param name="codeColumn">Tên cột chứa mã (ví dụ: customer_code)</param>
        /// <param name="prefix">Tiền tố (ví dụ: KH)</param>
        /// <param name="yearMonth">Năm tháng (ví dụ: 202411)</param>
        /// <returns>Số thứ tự tiếp theo (ví dụ: 123 cho KH202411000123)</returns>
        /// <remarks>CreatedBy: NTT (15/11/2025)</remarks>
        public virtual int GetNextSequenceNumber(string codeColumn, string prefix, string yearMonth)
        {
            var tableName = GetTableName();
            var pattern = $"{prefix}{yearMonth}%";

            // Lấy mã lớn nhất theo thứ tự giảm dần
            var sql = $@"SELECT {codeColumn} FROM {tableName} WHERE {codeColumn} LIKE @Pattern
                      ORDER BY {codeColumn} DESC LIMIT 1";

            var parameters = new DynamicParameters();
            parameters.Add("@Pattern", pattern);

            var lastCode = dbConnection.QueryFirstOrDefault<string>(sql, parameters);

            if (string.IsNullOrEmpty(lastCode))
            {
                // Chưa có mã nào, bắt đầu từ 1
                return 1;
            }

            // Lấy 6 số cuối từ mã hiện tại
            var lastNumber = lastCode.Substring(lastCode.Length - 6);
            if (int.TryParse(lastNumber, out var currentNumber))
            {
                return currentNumber + 1;
            }

            // Fallback nếu không parse được
            return 1;
        }



    }
}
