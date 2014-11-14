namespace Brainary.Commons.Unity
{
    using System;
    using System.Linq;

    using Microsoft.Practices.Unity.InterceptionExtension;

    public abstract class Interceptor
    {
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
