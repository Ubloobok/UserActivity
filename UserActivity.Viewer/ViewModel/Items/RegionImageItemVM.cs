using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;

namespace UserActivity.Viewer.ViewModel.Items
{
	public class RegionImageItemVM
	{
		public string RegionName { get; set; }
		public Image Image { get; set; }

		public string ImageName
		{
			get
			{
				string imageName = Image == null ? null : Image.Name;
				return imageName;
			}
		}

		public string DisplayName
		{
			get
			{
				string displayName =
					Image == null ?
					RegionName :
					string.Format("{0} ({1}x{2})", RegionName, Image.Width, Image.Height);
				return displayName;
			}
		}
	}
}
