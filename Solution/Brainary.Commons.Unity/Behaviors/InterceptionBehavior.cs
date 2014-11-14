namespace Brainary.Commons.Unity.Behaviors
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Practices.Unity.InterceptionExtension;

    public abstract class InterceptionBehavior : Unity.Interceptor, IInterceptionBehavior
    {
        public virtual bool WillExecute
        {
            get { return true; }
        }

        public virtual IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return ShouldCleanInvoke(input) ? CleanInvoke(input, getNext) : getNext()(input, getNext);
        }

        public abstract IMethodReturn CleanInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

        public virtual IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }
    }
}
