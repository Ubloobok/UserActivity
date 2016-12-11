using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserActivity.CL.WPF.Entities;

namespace UserActivity.CL.WPF.Services
{
	public interface IUserActivityService
	{
		Guid? CurrentSessionUID { get; }
		DateTime? CurrentSessionStartDateTime { get; }

		void OpenSession();
		void CloseAndOpenSession();
		void CloseSession();

		void RegisterActivity(ActivityInfo activityInfo);
	}
}
