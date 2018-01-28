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
	public partial class Activity
	{
		public const string DateTimeFormat = "yyyy.MM.dd HH:mm:ss";

		[XmlIgnore]
		public DateTime? UtcDateTime
		{
			get;
			set;
		}

        [XmlIgnore]
        public DateTime? LocalDateTime => UtcDateTime.HasValue ? UtcDateTime.Value.ToLocalTime() : (DateTime?)null;

        [XmlAttribute("UtcDateTime")]
		public string UtcDateTimeString
		{
			get { return UtcDateTime.HasValue ? UtcDateTime.Value.ToString(DateTimeFormat) : null; }
			set { UtcDateTime = string.IsNullOrEmpty(value) ? null : (DateTime?)DateTime.ParseExact(value, DateTimeFormat, CultureInfo.CurrentCulture); }
		}

		[XmlAttribute]
		public ActivityKind Kind
		{
			get;
			set;
		}

		[XmlIgnore]
		public double? InRegionX
		{
			get;
			set;
		}

		[XmlAttribute("InRegionX")]
		public string InRegionXString
		{
			get { return InRegionX.HasValue ? InRegionX.Value.ToString() : null; }
			set { InRegionX = string.IsNullOrEmpty(value) ? null : (double?)double.Parse(value); }
		}

		[XmlIgnore]
		public double? InRegionY
		{
			get;
			set;
		}

		[XmlAttribute("InRegionY")]
		public string InRegionYString
		{
			get { return InRegionY.HasValue ? InRegionY.Value.ToString() : null; }
			set { InRegionY = string.IsNullOrEmpty(value) ? null : (double?)double.Parse(value); }
		}

		[XmlAttribute]
		public string RegionName
		{
			get;
			set;
		}

		[XmlAttribute]
		public string ImageName
		{
			get;
			set;
		}

		[XmlIgnore]
		public double RegionWidth
		{
			get;
			set;
		}

		[XmlIgnore]
		public double RegionHeight
		{
			get;
			set;
		}

		[XmlIgnore]
		public Region Region
		{
			get;
			set;
		}
	}
}
