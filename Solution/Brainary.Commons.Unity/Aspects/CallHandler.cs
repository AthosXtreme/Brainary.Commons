namespace Brainary.Commons.Unity.Aspects
{
    using Microsoft.Practices.Unity.InterceptionExtension;

    public abstract class CallHandler : Unity.Interceptor, ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            return ShouldCleanInvoke(input) ? CleanInvoke(input, getNext) : getNext()(input, getNext);
        }

        public abstract IMethodReturn CleanInvoke(IMethodInvocation input, GetNextHandlerDelegate getNext);

    }
}
