using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UserActivity.CL.WPF.Entities
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class Session
	{
		public const string DateTimeFormat = "yyyy.MM.dd HH:mm:ss";

		public Session()
		{
			ActivityCollection = new List<Activity>();
			RegionCollection = new List<Region>();
		}

		[XmlAttribute]
		public string UID
		{
			get;
			set;
		}

		[XmlIgnore]
		public DateTime? UtcStartDateTime
		{
			get;
			set;
		}

		[XmlAttribute("UtcStartDateTime")]
		public string UtcStartDateTimeString
		{
			get { return UtcStartDateTime.HasValue ? UtcStartDateTime.Value.ToString(DateTimeFormat) : null; }
			set { UtcStartDateTime = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.ParseExact(value, DateTimeFormat, CultureInfo.CurrentCulture); }
		}

		[XmlIgnore]
		public DateTime? UtcEndDateTime
		{
			get;
			set;
		}

		[XmlAttribute("UtcEndDateTime")]
		public string UtcEndDateTimeString
		{
			get { return UtcEndDateTime.HasValue ? UtcStartDateTime.Value.ToString(DateTimeFormat) : null; }
			set { UtcEndDateTime = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.ParseExact(value, DateTimeFormat, CultureInfo.CurrentCulture); }
		}

		[XmlAttribute]
		public sbyte UtcShift
		{
			get;
			set;
		}

		[XmlArrayItemAttribute("Region", IsNullable = true)]
		public List<Region> RegionCollection
		{
			get;
			private set;
		}

		[XmlArrayItemAttribute("Activity", IsNullable = true)]
		public List<Activity> ActivityCollection
		{
			get;
			private set;
		}
	}
}
