namespace MISA.CRM.Core.Options
{
    /// <summary>
    /// Options c?u hình cho File Storage (dùng Options pattern)
    /// </summary>
    /// <remarks>CreatedBy: NTT (18/11/2025)</remarks>
    public class FileStorageOptions
    {
        /// <summary>
        /// Th? m?c g?c dùng ?? l?u file (có th? là ???ng d?n t??ng ??i nh? "wwwroot" ho?c ???ng d?n tuy?t ??i)
        /// </summary>
        public string? RootPath { get; set; }
    }
}
