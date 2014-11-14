﻿namespace Brainary.Commons.Unity.Behaviors
{
    using Microsoft.Practices.Unity.InterceptionExtension;

    public class ExceptionLogBehavior : Behavior
    {
        public ExceptionLogBehavior(ILogger logger)
            : base(logger)
        {
        }

        public override IMethodReturn CleanInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var targetType = GetRealTargetType(input);
            var result = getNext()(input, getNext);
            if (result.Exception != null)
                Logger.Error(string.Format(Messages.ExceptionAt, targetType.Name, input.MethodBase.Name), result.Exception);

            return result;
        }
    }
}