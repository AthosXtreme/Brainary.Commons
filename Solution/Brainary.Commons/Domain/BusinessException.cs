using System.Runtime.Serialization;

namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Represents a business logic exception
    /// </summary>
    public class BusinessException : Exception
	{
		public BusinessException()
		{
		}

		public BusinessException(string message)
			: base(message)
		{
		}

		public BusinessException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected BusinessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	} 
}
