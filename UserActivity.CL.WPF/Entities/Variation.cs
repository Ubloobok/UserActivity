using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UserActivity.CL.WPF.Entities
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class Variation
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public ImageType ImageType { get; set; }

        [XmlAttribute]
        public double ImageDpiX { get; set; }

        [XmlAttribute]
        public double ImageDpiY { get; set; }

        [XmlAttribute]
        public double Width { get; set; }

        [XmlAttribute]
        public double Height { get; set; }

        [XmlIgnore]
        public string Source { get; set; }

        [XmlElement(ElementName = "Source")]
        public XmlCDataSection SourceSection
        {
            get { return new XmlDocument().CreateCDataSection(Source ?? string.Empty); }
            set { Source = value.Value; }
        }

        [XmlIgnore]
        public byte[] Data { get; set; }

        [XmlElement(ElementName = "Data")]
        public XmlCDataSection DataSection
        {
            get
            {
                string dataString = Data == null ? string.Empty : Convert.ToBase64String(Data);
                return new XmlDocument().CreateCDataSection(dataString);
            }
            set
            {
                string dataString = value.Value;
                Data = Convert.FromBase64String(dataString);
            }
        }
    }
}
