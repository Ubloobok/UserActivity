using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserActivity.CL.WPF.Entities
{
    public class EventInfo
    {
        public EventInfo()
        {
        }

        public EventInfo(string regionName, double inRegionX, double inRegionY, double regionWidth, double regionHeight, EventKind kind, string commandName = null)
        {
            Kind = kind;
            InRegionX = inRegionX;
            InRegionY = inRegionY;
            RegionName = regionName;
            RegionWidth = regionWidth;
            RegionHeight = regionHeight;
            CommandName = commandName;
        }

        public EventKind Kind { get; set; }
        public double InRegionX { get; set; }
        public double InRegionY { get; set; }
        public string RegionName { get; set; }
        public double RegionWidth { get; set; }
        public double RegionHeight { get; set; }
        public string CommandName { get; set; }

        public Func<Variation> CreateRegionImage { get; set; }
    }
}
