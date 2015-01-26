namespace Brainary.Commons.Web.Annotations
{
    public class HttpDeleteAttribute : HttpVerbAttribute
    {
        public override string HttpVerb
        {
            get { return "DELETE"; }
        }
    }
}