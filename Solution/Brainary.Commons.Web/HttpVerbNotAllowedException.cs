namespace Brainary.Commons.Web
{
    using System;

    /// <summary>
    /// Represents a not allowed HTTP verb exception
    /// </summary>
    public class HttpVerbNotAllowedException : Exception
    {
        public HttpVerbNotAllowedException()
        {
        }

        public HttpVerbNotAllowedException(string message)
            : base(message)
        {
        }

        public HttpVerbNotAllowedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected HttpVerbNotAllowedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
