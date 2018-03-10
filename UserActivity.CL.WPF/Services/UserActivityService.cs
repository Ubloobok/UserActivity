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
        private static readonly object _syncObj = new object();
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

        public Guid? CurrentSessionUID { get; protected set; }

        public DateTime? CurrentSessionStartDateTime { get; protected set; }

        public IUserActivityDataContext CurrentDataContext { get; protected set; }

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

        public void RegisterEvent(EventInfo evInfo)
        {
            if (CurrentSessionUID.HasValue)
            {
                var ev = new Event()
                {
                    UtcDateTime = DateTime.Now.ToUniversalTime(),
                    Kind = evInfo.Kind,
                    InRegionX = evInfo.InRegionX,
                    InRegionY = evInfo.InRegionY,
                    RegionName = evInfo.RegionName,
                    ImageName = string.Format("{0}x{1}", evInfo.RegionWidth, evInfo.RegionHeight),
                    RegionWidth = evInfo.RegionWidth,
                    RegionHeight = evInfo.RegionHeight,
                    Region = new Region() { Name = evInfo.RegionName },
                    CommandName = evInfo.CommandName,
                };

                if (!CurrentDataContext.GetIsRegionImageExist(ev.RegionName, ev.ImageName))
                {
                    var regionImage = evInfo.CreateRegionImage();
                    regionImage.Name = ev.ImageName;
                    ev.Region.Variations.Add(regionImage);
                }

                CurrentDataContext.WriteEvent(CurrentSessionUID.Value, ev);
            }
        }
    }
}
