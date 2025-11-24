using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using MISA.CRM.Core.MISAAtributes;
using System.ComponentModel.DataAnnotations;

namespace MISA.CRM.Core.Entities
{
    /// <summary>
    /// Thực entity khách hàng (Customer) biểu diễn bảng dữ liệu khách hàng trong database.
    /// </summary>
    /// Created By: NTT (15/11/2025)
    [TableName("customer")]
    public class Customer
    {
        /// <summary>
        /// Khóa chính khách hàng
        /// </summary>
        [PrimaryKey]
        [Column("customer_id")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Mã khách hàng tự sinh (KHyyyyMMxxxxxx)
        /// </summary>
        [AutoGenerateCode("KH")]
        [Column("customer_code")]
        [MaxLength(20)]
        public string? CustomerCode { get; set; }

        /// <summary>
        /// Họ và tên khách hàng
        /// </summary>
        [NotEmpty("Tên khách hàng không được để trống")]
        [MaxLengthCustom(128, "Tên khách hàng không được vượt quá 128 ký tự")]
        [Column("customer_full_name")]
        public string? CustomerFullName { get; set; }

        /// <summary>
        /// Mã số thuế khách hàng
        /// </summary>
        [Column("customer_tax_code")]
        public string? CustomerTaxCode { get; set; }

        /// <summary>
        /// Email khách hàng
        /// </summary>
        [EmailFormat("Email không đúng định dạng")]
        [Unique("Email khách hàng đã tồn tại trong hệ thống")]
        [Column("customer_email")]
        public string? CustomerEmail { get; set; }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
        [PhoneLength("Số điện thoại phải có 10-11 chữ số")]
        [Unique("Số điện thoại khách hàng đã tồn tại trong hệ thống")]
        [Column("customer_phone")]
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Loại khách hàng (VIP, Thường, Doanh nghiệp, v.v.)
        /// </summary>
        [Column("customer_type")]
        public string? CustomerType { get; set; }

        /// <summary>
        /// Địa chỉ giao hàng
        /// </summary>
        [MaxLengthCustom(255, "Địa chỉ giao hàng không được vượt quá 255 ký tự")]
        [Column("customer_shipping_addr")]
        public string? CustomerShippingAddr { get; set; }

        /// <summary>
        /// Ngày mua gần nhất
        /// </summary>
        [Column("customer_last_purchase_date")]
        public DateTime? CustomerLastPurchaseDate { get; set; }

        /// <summary>
        /// Hàng hóa tổng quát
        /// </summary>
        [Column("customer_purchased_items")]
        public string? CustomerPurchasedItems { get; set; }

        /// <summary>
        /// Hàng hóa mua gần nhất
        /// </summary>
        [Column("customer_lastest_purchased_items")]
        public string? CustomerLastestPurchasedItems { get; set; }

        /// <summary>
        /// Đánh dấu bản ghi đã bị xóa hay chưa (soft delete)
        /// </summary>
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Đường dẫn hoặc url avatar khách hàng (có thể null)
        /// </summary>
        [Column("customer_avatar")]
        public string? CustomerAvatar { get; set; }
    }
}
