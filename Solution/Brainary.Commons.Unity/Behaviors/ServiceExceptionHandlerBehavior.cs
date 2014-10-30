namespace Brainary.Commons.Unity.Behaviors
{
    using System.ServiceModel;

    using Brainary.Commons.Domain;

    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ServiceExceptionHandlerBehavior : Behavior
    {
        public ServiceExceptionHandlerBehavior(ILogger logger)
            : base(logger)
        {
        }

        public override IMethodReturn CleanInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var result = getNext()(input, getNext);
            if (result.Exception != null)
                throw new FaultException<ServiceError>(new ServiceError(result.Exception));

            return result;
        }
    }
}
