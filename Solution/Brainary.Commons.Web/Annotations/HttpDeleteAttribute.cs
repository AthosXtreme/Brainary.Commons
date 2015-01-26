namespace Brainary.Commons.Web.Annotations
{
    public class HttpPutAttribute : HttpVerbAttribute
    {
        public override string HttpVerb
        {
            get { return "PUT"; }
        }
    }
}