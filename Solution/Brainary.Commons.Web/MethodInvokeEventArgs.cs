namespace Brainary.Commons.Web
{
    using System.ComponentModel;
    using System.Reflection;

    public class MethodInvokeEventArgs : CancelEventArgs
    {
        protected internal MethodInvokeEventArgs(MethodInfo method)
        {
            Method = method;
        }

        public MethodInfo Method { get; private set; }
    }
}
