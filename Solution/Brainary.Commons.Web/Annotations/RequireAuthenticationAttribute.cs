namespace Brainary.Commons.Web.Annotations
{
    using System;

    public class RequireAuthenticationAttribute : Attribute
    {
        public readonly bool RequireAuthentication = false;

        public RequireAuthenticationAttribute(bool value)
        {
            RequireAuthentication = value;
        }
    }
}
