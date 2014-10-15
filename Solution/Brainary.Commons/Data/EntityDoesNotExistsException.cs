namespace Brainary.Commons.Data
{
    using System;

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

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected EntityDoesNotExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}