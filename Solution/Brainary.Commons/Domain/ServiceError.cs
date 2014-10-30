namespace Brainary.Commons.Domain
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class to return service exceptions using <see cref="System.ServiceModel.FaultContractAttribute"/>
    /// </summary>
    [DataContract]
    public class ServiceError
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exception">Thrown exception</param>
        public ServiceError(Exception exception)
        {
            ErrorMessage = exception.Message;
            ExceptionType = exception.GetType().FullName;

            if (exception.InnerException != null)
                InnerServiceError = new ServiceError(exception.InnerException);
        }

        /// <summary>
        /// Error message
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Exception type name
        /// </summary>
        [DataMember]
        public string ExceptionType { get; set; }

        /// <summary>
        /// Inner exception level
        /// </summary>
        [DataMember]
        public ServiceError InnerServiceError { get; set; }
    }
}
