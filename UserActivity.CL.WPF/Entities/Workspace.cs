using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserActivity.CL.WPF.Entities
{
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.33440")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class Workspace
	{
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ComputerName
		{
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ComputerWorkgroup
		{
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string ComputerIpAddress
		{
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string UserDomainName
		{
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string UserAppName
		{
			get;
			set;
		}
	}
}
