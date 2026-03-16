using System;

namespace shumilovskih_library.Exceptions
{
    /// <summary>
    /// Исключение для ошибок работы с партнёрами
    /// </summary>
    public class PartnerException : Exception
    {
        public PartnerException() : base()
        {
        }

        public PartnerException(string message) : base(message)
        {
        }

        public PartnerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}