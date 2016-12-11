using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserActivity.CL.WPF.Entities;

namespace UserActivity.CL.WPF.Services
{
	public class UserActivityService : IUserActivityService
	{
		private static readonly object _syncObj =new object();
		private static IUserActivityService _current;

		static UserActivityService()
		{
			var defaultDataContext = new DebugUserActivityDataContext();
			Initialize(defaultDataContext);
		}

		public static IUserActivityService Current
		{
			get { lock (_syncObj) return _current; }
			set { lock (_syncObj) _current = value; }
		}

		public static void Initialize(IUserActivityDataContext dataContext)
		{
			var service = new UserActivityService(dataContext);
			Current = service;
		}

		public UserActivityService(IUserActivityDataContext dataContext)
		{
			if (dataContext == null)
			{
				throw new ArgumentNullException();
			}
			CurrentDataContext = dataContext;
		}

		public Guid? CurrentSessionUID
		{
			get;
			protected set;
		}

		public DateTime? CurrentSessionStartDateTime
		{
			get;
			protected set;
		}

		public IUserActivityDataContext CurrentDataContext
		{
			get;
			protected set;
		}

		public void OpenSession()
		{
			if (CurrentSessionUID.HasValue)
			{
				CloseSession();
			}
			CurrentSessionUID = Guid.NewGuid();
			CurrentSessionStartDateTime = DateTime.Now;
			CurrentDataContext.OpenSession(CurrentSessionUID.Value, CurrentSessionStartDateTime.Value);
		}

		public void CloseAndOpenSession()
		{
			if (CurrentSessionUID.HasValue)
			{
				CloseSession();
			}
			OpenSession();
		}

		public void CloseSession()
		{
			if (CurrentSessionUID.HasValue)
			{
				CurrentDataContext.CloseSession(CurrentSessionUID.Value, DateTime.Now);
				CurrentSessionUID = null;
				CurrentSessionStartDateTime = null;
			}
		}

		public void RegisterActivity(ActivityInfo activityInfo)
		{
			if (CurrentSessionUID.HasValue)
			{
				var activity = new Activity()
				{
					UtcDateTime = DateTime.Now.ToUniversalTime(),
					Kind = activityInfo.GlobalType,
					InRegionX = activityInfo.InRegionX,
					InRegionY = activityInfo.InRegionY,
					RegionName = activityInfo.RegionName,
					ImageName = string.Format("{0}x{1}", activityInfo.RegionWidth, activityInfo.RegionHeight),
					RegionWidth = activityInfo.RegionWidth,
					RegionHeight = activityInfo.RegionHeight,
					Region = new Region() { Name = activityInfo.RegionName },
				};

				if (!CurrentDataContext.GetIsRegionImageExist(activity.RegionName, activity.ImageName))
				{
					var regionImage = activityInfo.CreateRegionImage();
					regionImage.Name = activity.ImageName;
					activity.Region.Images.Add(regionImage);
				}

				CurrentDataContext.WriteActivity(CurrentSessionUID.Value, activity);
			}
		}
	}
}
