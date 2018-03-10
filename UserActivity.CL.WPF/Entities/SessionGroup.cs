using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UserActivity.CL.WPF.Entities
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class SessionGroup
    {
        [XmlArrayItem("Session", IsNullable = true)]
        public List<Session> Sessions { get; private set; } = new List<Session>();
    }
}
