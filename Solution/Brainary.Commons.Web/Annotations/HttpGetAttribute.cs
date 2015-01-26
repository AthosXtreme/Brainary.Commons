namespace Brainary.Commons.Web.Annotations
{
    public class HttpGetAttribute : HttpVerbAttribute
    {
        public override string HttpVerb
        {
            get { return "GET"; }
        }
    }
}