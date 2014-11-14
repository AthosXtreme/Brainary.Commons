namespace Brainary.Commons.Unity.Behaviors
{
    using System.ServiceModel;

    using Brainary.Commons.Domain;

    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ServiceExceptionHandlerBehavior : InterceptionBehavior
    {
        public override IMethodReturn CleanInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var result = getNext()(input, getNext);
            if (result.Exception != null)
                throw new FaultException<ServiceError>(new ServiceError(result.Exception), new FaultReason(result.Exception.Message), new FaultCode(result.Exception.GetType().Name, result.Exception.GetType().Namespace));

            return result;
        }
    }
}
