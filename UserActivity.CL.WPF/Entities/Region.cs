using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UserActivity.CL.WPF.Entities
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class Region
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlArrayItem("Variation", IsNullable = true)]
        public List<Variation> Variations { get; private set; } = new List<Variation>();
    }
}
