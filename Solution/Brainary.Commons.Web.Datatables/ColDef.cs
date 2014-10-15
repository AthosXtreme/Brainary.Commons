namespace Brainary.Commons.Web.Datatables
{
    using System;

    public class ColDef
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public Type Type { get; set; }

        public bool Hidden { get; set; }

        public bool Sortable { get; set; }

        public static ColDef Create(string name, string p1, Type propertyType, bool sortable = true)
        {
            return new ColDef { Name = name, DisplayName = p1, Type = propertyType, Sortable = sortable };
        }
    }
}