namespace Brainary.Commons.Web.DataTables
{
    public class RequestColumn
    {
        public string Data { get; set; }

        public string Name { get; set; }

        public bool Searchable { get; set; }

        public bool Orderable { get; set; }

        public RequestSearch Search { get; set; }
    }
}