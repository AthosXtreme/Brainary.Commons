namespace Brainary.Commons.Web.DataTables
{
    using System.Collections.Generic;

    public class RequestParameters
    {
        public RequestParameters()
        {
            Order = new List<RequestOrder>();
            Columns = new List<RequestColumn>();
        }

        public int Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public RequestSearch Search { get; set; }

        public IList<RequestOrder> Order { get; set; }

        public IList<RequestColumn> Columns { get; set; }

        public IDictionary<string, string> CustomData { get; set; }
    }
}