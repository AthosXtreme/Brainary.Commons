using System.Runtime.Serialization;

namespace Brainary.Commons.Domain
{
	public class EntityDoesNotExistsException : Exception
	{
		public EntityDoesNotExistsException()
		{
		}

		public EntityDoesNotExistsException(string message)
			: base(message)
		{
		}

		public EntityDoesNotExistsException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected EntityDoesNotExistsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	} 
}
