namespace Brainary.Commons.Data.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Brainary.Commons.Extensions;

    /// <summary>
    /// Handle and serialize a list of <see cref="XmlParameter"/>
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Parameters")]
    public class XmlParameterList
    {
        [XmlElement(ElementName = "Param")]
        public List<XmlParameter> Parameters { get; set; }

        public string ToXml()
        {
            return this.Serialize();
        }
    }
}