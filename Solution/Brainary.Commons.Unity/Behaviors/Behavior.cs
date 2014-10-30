namespace Brainary.Commons.Unity.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public virtual IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return ShouldCleanInvoke(input) ? CleanInvoke(input, getNext) : getNext()(input, getNext);
        }

        public abstract IMethodReturn CleanInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext);

        public virtual IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        protected static Type GetRealTargetType(IMethodInvocation input)
        {
            var type = input.Target.GetType();
            while (type != null && type.Namespace == "DynamicModule.ns") type = type.BaseType;
            return type;
        }

        protected static bool IsInterfaceImplementation(IMethodInvocation input)
        {
            var implementedType = input.MethodBase.ReflectedType;
            if (implementedType == null || !implementedType.IsInterface) return false;

            var targetType = GetRealTargetType(input);
            return targetType.GetInterfaces().Contains(implementedType);
        }

        protected static bool ShouldCleanInvoke(IMethodInvocation input)
        {
            return IsInterfaceImplementation(input)
                   || GetRealTargetType(input).Assembly == input.MethodBase.Module.Assembly;
        }
    }
}
