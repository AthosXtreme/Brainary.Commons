using System.Runtime.Serialization;

namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Represents an exception for an already existing entity
    /// </summary>
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
