namespace Brainary.Commons.Web.DataTables
{
    using System;

    public class ColInfo
    {
        public ColInfo(string name, Type propertyType)
        {
            Name = name;
            Type = propertyType;
        }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}