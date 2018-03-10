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
        void OpenSession(Guid sessionUID, DateTime start);
        void CloseSession(Guid sessionUID, DateTime end);
        void WriteEvent(Guid sessionUID, Event ev);
        bool GetIsRegionImageExist(string regionName, string imageName);
    }
}
