using System.Runtime.Serialization;

namespace Brainary.Commons.Domain
{
	public class EntityAlreadyExistsException : Exception
	{
		public EntityAlreadyExistsException()
		{
		}

		public EntityAlreadyExistsException(string message)
			: base(message)
		{
		}

		public EntityAlreadyExistsException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected EntityAlreadyExistsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	} 
}
