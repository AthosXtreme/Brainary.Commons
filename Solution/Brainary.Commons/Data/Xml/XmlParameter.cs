namespace Brainary.Commons.Data.Xml
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Class intended to pass xml serialized parameters to database programs
    /// </summary>
    [Serializable]
    public class XmlParameter
    {
        public XmlParameter(string name, object value)
        {
            Name = name;

            if (value is DateTime)
            {
                var dat = (DateTime)value;
                Value = dat.ToString("yyyyMMdd HH:mm:ss");
            }
            else
            {
                Value = value.ToString();
            }
        }

        private XmlParameter()
        {
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}