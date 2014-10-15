namespace Brainary.Commons.Unity.Behaviors
{
    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ExceptionLogBehavior : Behavior
    {
        public ExceptionLogBehavior(ILogger logger)
            : base(logger)
        {
        }

        public override IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var targetType = GetImplementingType(input);
            if (targetType.Assembly != input.MethodBase.Module.Assembly) return getNext()(input, getNext);

            var result = getNext()(input, getNext);
            if (result.Exception != null)
                Logger.Error(string.Format(Messages.ExceptionAt, targetType.Name, input.MethodBase.Name), result.Exception);

            return result;
        }
    }
}
