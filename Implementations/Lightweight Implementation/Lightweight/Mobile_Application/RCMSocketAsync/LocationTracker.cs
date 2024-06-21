using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MobileClient
{
    public class LocationTracker
    {
        LogicalLocationFinder LogicalLocFinder;
        PhysicalLocationTracker PhysicalTracker;
        
        public LocationTracker()
        {
            LogicalLocFinder = new LogicalLocationFinder();
            PhysicalTracker = new PhysicalLocationTracker();
        }

        public int getLocationID(int locationChoice)
        {
            int location = -1;
            PhysicalLocation userLocation = PhysicalTracker.getPhysicalLocation();
            try
            {
                if (userLocation != null)
                {
                    location = LogicalLocFinder.findLogicalLocation(userLocation);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            return location;
        }
    }
}
