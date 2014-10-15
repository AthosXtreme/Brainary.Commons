namespace Brainary.Commons.Unity.Aspects
{
    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ExceptionLogInterceptor : IExceptionLogInterceptor
    {
        private readonly ILogger logger;

        public ExceptionLogInterceptor(ILogger logger)
        {
            this.logger = logger;
        }

        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var result = getNext()(input, getNext);
            if (result.Exception != null) 
                logger.Error(string.Format(Messages.ExceptionAt, input.Target.GetType().Name, input.MethodBase.Name), result.Exception);

            return result;
        }
    }
}
