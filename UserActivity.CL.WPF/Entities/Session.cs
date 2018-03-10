using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UserActivity.CL.WPF.Entities
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class Session
    {
        public const string DateTimeFormat = "yyyy.MM.dd HH:mm:ss";

        [XmlAttribute]
        public string UID { get; set; }

        [XmlIgnore]
        public DateTime? UtcStartDateTime { get; set; }

        [XmlAttribute("UtcStartDateTime")]
        public string UtcStartDateTimeString
        {
            get { return UtcStartDateTime.HasValue ? UtcStartDateTime.Value.ToString(DateTimeFormat) : null; }
            set { UtcStartDateTime = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.ParseExact(value, DateTimeFormat, CultureInfo.CurrentCulture); }
        }

        [XmlIgnore]
        public DateTime? UtcEndDateTime { get; set; }

        [XmlAttribute("UtcEndDateTime")]
        public string UtcEndDateTimeString
        {
            get { return UtcEndDateTime.HasValue ? UtcStartDateTime.Value.ToString(DateTimeFormat) : null; }
            set { UtcEndDateTime = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.ParseExact(value, DateTimeFormat, CultureInfo.CurrentCulture); }
        }

        [XmlAttribute]
        public sbyte UtcShift { get; set; }

        [XmlArrayItem("Region", IsNullable = true)]
        public List<Region> Regions { get; private set; } = new List<Region>();

        [XmlArrayItem("Event", IsNullable = true)]
        public List<Event> Events { get; private set; } = new List<Event>();
    }
}
