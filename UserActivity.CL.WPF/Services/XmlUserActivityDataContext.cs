using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UserActivity.CL.WPF.Entities;

namespace UserActivity.CL.WPF.Services
{
	public class XmlUserActivityDataContext : IUserActivityDataContext
	{
		public const string UadFileExtension = "uad";

		public SessionGroup CurrentSessionGroup { get; private set; }

		public Session CurrentSession { get; private set; }

		public void OpenSession(Guid sessionUID, DateTime startDateTime)
		{
			var utcShiftSpan = startDateTime - startDateTime.ToUniversalTime();
			sbyte utcShift = (sbyte)utcShiftSpan.Hours;

			CurrentSessionGroup = new SessionGroup();
			CurrentSession = new Session()
			{
				UID = sessionUID.ToString().ToUpper(),
				UtcStartDateTime = startDateTime.ToUniversalTime(),
				UtcShift = utcShift,
			};
			CurrentSessionGroup.Sessions.Add(CurrentSession);
		}

		public void CloseSession(Guid sessionUID, DateTime endDateTime)
		{
			CurrentSession.UtcEndDateTime = endDateTime.ToUniversalTime();

			var serializer = new XmlSerializer(typeof(SessionGroup));
			string fileName = CurrentSession.UID + "." + UadFileExtension;
			using (var fileStream = File.Open(fileName, FileMode.OpenOrCreate))
			{
				serializer.Serialize(fileStream, CurrentSessionGroup);
			}

			CurrentSession = null;
			CurrentSessionGroup = null;
		}

		public void WriteActivity(Guid sessionUID, Activity activity)
		{
			var region = CurrentSession.RegionCollection.FirstOrDefault(r => r.Name == activity.RegionName);
			if (region == null)
			{
				region = activity.Region;
				CurrentSession.RegionCollection.Add(region);
			}
			else
			{
				var oldImage = region.Images.FirstOrDefault(i => i.Name == activity.RegionName);
				var newImage = activity.Region.Images.FirstOrDefault();
				if (newImage != null)
				{
					if (oldImage == null)
					{
						region.Images.Add(newImage);
					}
					else
					{
						region.Images.Remove(oldImage);
						region.Images.Add(newImage);
					}
				}
			}

			CurrentSession.ActivityCollection.Add(activity);
		}

		public bool GetIsRegionImageExist(string regionName, string imageName)
		{
			bool isExist = false;

			var region = CurrentSession.RegionCollection.FirstOrDefault(r => r.Name == regionName);
			if (region != null)
			{
				var image = region.Images.FirstOrDefault(i => i.Name == imageName);
				isExist = image != null;
			}

			return isExist;
		}

		public static SessionGroup LoadSessionGroup(Stream xmlStream)
		{
			SessionGroup sessionGroup = null;

			var serializer = new XmlSerializer(typeof(SessionGroup));
			using (xmlStream)
			{
				sessionGroup = (SessionGroup)serializer.Deserialize(xmlStream);
			}

			return sessionGroup;
		}
	}
}
