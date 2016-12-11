using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;

namespace UserActivity.CL.WPF.Services
{
	public interface IUserActivityDataContext
	{
		void OpenSession(Guid sessionUID, DateTime startDateTime);
		void CloseSession(Guid sessionUID, DateTime endDateTime);
		void WriteActivity(Guid sessionUID, Activity activity);
		bool GetIsRegionImageExist(string regionName, string imageName);
	}
}
