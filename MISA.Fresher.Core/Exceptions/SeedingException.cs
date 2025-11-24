using System;

namespace MISA.CRM.Core.Exceptions
{
  /// <summary>
  /// Ngo?i l? ???c ném ra khi có l?i trong quá trình seeding d? li?u.
    /// </summary>
    public class SeedingException : Exception
    {
      /// <summary>
 /// Kh?i t?o SeedingException v?i thông ?i?p c? th?.
        /// </summary>
        /// <param name="message">Thông ?i?p l?i.</param>
        public SeedingException(string message) : base(message)
        {
        }

  /// <summary>
        /// Kh?i t?o SeedingException v?i thông ?i?p và inner exception.
        /// </summary>
        /// <param name="message">Thông ?i?p l?i.</param>
     /// <param name="innerException">Exception g?c.</param>
        public SeedingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}