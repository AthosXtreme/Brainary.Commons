using System.Runtime.Serialization;

namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Represents a business logic exception
    /// </summary>
    public class BusinessException : Exception
    {
        public int Code { get; set; } = -1;

        public BusinessException()
        {
        }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(int code, string message)
            : base(message)
        {
            Code = code;
        }

        public BusinessException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public BusinessException(int code, string message, Exception inner)
            : base(message, inner)
        {
            Code = code;
        }
    }
}
