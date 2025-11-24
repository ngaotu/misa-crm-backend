using System;
using System.Collections.Generic;

namespace MISA.CRM.Core.DTOs.Customer
{
 /// <summary>
 /// DTO cho API gán loại khách hàng
 /// </summary>
 /// Created By: NTT (15/11/2025)
 public class BulkAssignTypeRequest
 {
 public List<Guid> CustomerIds { get; set; }
 public string? CustomerType { get; set; }
 }
}
