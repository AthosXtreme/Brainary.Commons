namespace Brainary.Commons.Web.Annotations
{
    public class HttpPostAttribute : HttpVerbAttribute
    {
        public override string HttpVerb
        {
            get { return "POST"; }
        }
    }
}