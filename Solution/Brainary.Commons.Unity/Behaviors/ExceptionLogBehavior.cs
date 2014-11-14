namespace Brainary.Commons.Unity.Behaviors
{
    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ExceptionLogBehavior : InterceptionBehavior
    {
        private readonly ILogger logger;

        public ExceptionLogBehavior(ILogger logger)
        {
            this.logger = logger;
        }

        public override IMethodReturn CleanInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var targetType = GetRealTargetType(input);
            var result = getNext()(input, getNext);
            if (result.Exception != null)
                logger.Error(string.Format(Messages.ExceptionAt, targetType.Name, input.MethodBase.Name), result.Exception);

            return result;
        }
    }
}
