using System.Runtime.Serialization;

namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Represents an exception for a non-existent entity
    /// </summary>
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
	} 
}
