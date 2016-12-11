using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace UserActivity.CL.WPF.Entities
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public class Image
	{
		[XmlAttribute]
		public string Name
		{
			get;
			set;
		}

		[XmlAttribute]
		public ImageType Type
		{
			get;
			set;
		}

		[XmlAttribute]
		public double DpiX
		{
			get;
			set;
		}

		[XmlAttribute]
		public double DpiY
		{
			get;
			set;
		}

		[XmlAttribute]
		public double Width
		{
			get;
			set;
		}

		[XmlAttribute]
		public double Height
		{
			get;
			set;
		}

		[XmlIgnore]
		public string Source
		{
			get;
			set;
		}

		[XmlElement(ElementName = "Source")]
		public XmlCDataSection SourceSection
		{
			get
			{
				return new XmlDocument().CreateCDataSection(Source ?? string.Empty);
			}
			set
			{
				Source = value.Value;
			}
		}

		[XmlIgnore]
		public byte[] Data
		{
			get;
			set;
		}

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
