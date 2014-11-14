namespace Brainary.Commons.Unity.Aspects
{
    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ExceptionLogInterceptor : CallHandler, IExceptionLogInterceptor
    {
        private readonly ILogger logger;

        public ExceptionLogInterceptor(ILogger logger)
        {
            this.logger = logger;
        }
        
        public override IMethodReturn CleanInvoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var targetType = GetRealTargetType(input);
            var result = getNext()(input, getNext);
            if (result.Exception != null)
                logger.Error(string.Format(Messages.ExceptionAt, targetType.Name, input.MethodBase.Name), result.Exception);

            return result;
        }
    }
}
