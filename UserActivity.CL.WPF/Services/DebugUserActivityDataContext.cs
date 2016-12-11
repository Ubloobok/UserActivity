using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;

namespace UserActivity.CL.WPF.Services
{
	public class DebugUserActivityDataContext : IUserActivityDataContext
	{
		public void OpenSession(Guid sessionUID, DateTime startDateTime)
		{
			Debug.WriteLine("UsabilityMetrics => Session Opened. UID: {0}, StartDateTime: {1}.", sessionUID, startDateTime);
		}

		public void CloseSession(Guid sessionUID, DateTime endDateTime)
		{
			Debug.WriteLine("UsabilityMetrics => Session Closed. UID: {0}, EndDateTime: {1}.", sessionUID, endDateTime);
		}

		public void WriteActivity(Guid sessionUID, Activity activity)
		{
			Debug.WriteLine("UsabilityMetrics => Session Event. UID: {0}, DateTime: {1}.", sessionUID, DateTime.Now);
			Debug.WriteLine("	GlobalType: {0}, RegionName: {1}, InRegionX: {2}, InRegionY: {3}.", activity.Kind, activity.RegionName ?? "<null>", activity.InRegionX, activity.InRegionY);
			//Debug.WriteLine("	Window: {0}x{1} ({2}, {3}), Screen: {4}x{5}.", windowInfo.Width, windowInfo.Height, windowInfo.X, windowInfo.Y, screenInfo.Width, screenInfo.Height);
		}

		public bool GetIsRegionImageExist(string regionName, string imageName)
		{
			return true;
		}
	}
}
