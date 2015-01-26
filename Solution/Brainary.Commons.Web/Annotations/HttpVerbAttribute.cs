namespace Brainary.Commons.Web.Annotations
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class HttpVerbAttribute : Attribute
    {
        public abstract string HttpVerb { get; }
    }
}