using System;
using System.Collections.Generic;
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
	public partial class Region
	{
		public Region()
		{
			Images = new List<Image>();
		}

		[XmlAttribute]
		public string Name
		{
			get;
			set;
		}

		[XmlArrayItemAttribute("Image", IsNullable = true)]
		public List<Image> Images
		{
			get;
			private set;
		}
	}
}
