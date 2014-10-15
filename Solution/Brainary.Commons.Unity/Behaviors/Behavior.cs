namespace Brainary.Commons.Unity.Behaviors
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Practices.Unity.InterceptionExtension;

    public abstract class Behavior : IInterceptionBehavior
    {
        protected readonly ILogger Logger;

        protected Behavior(ILogger logger)
        {
            Logger = logger;
        }

        public virtual bool WillExecute
        {
            get { return true; }
        }

        public abstract IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

        public virtual IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        protected static Type GetImplementingType(IMethodInvocation input)
        {
            var type = input.Target.GetType();
            while (type != null && type.Namespace == "DynamicModule.ns") type = type.BaseType;
            return type;
        }
    }
}
