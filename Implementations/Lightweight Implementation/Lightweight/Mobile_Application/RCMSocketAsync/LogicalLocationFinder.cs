using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MobileClient
{
    class LogicalLocationFinder
    {
        List<LocationPoint> locationList;

        public LogicalLocationFinder()
        {
            locationList = new List<LocationPoint>();

            locationList.Add(new LocationPoint("Home", 27.5548, -99.4413, .0002, .0002, .0002, .0002));
            locationList.Add(new LocationPoint("Development Office", 27.5721, -99.4355, .0001, .0001, .0004, .0003));
            locationList.Add(new LocationPoint("TestingOffice", 32.5007, -84.9342, .0003, .0004, .0001, .0001));
            locationList.Add(new LocationPoint("DirectorOffice", 27.5725, -99.4336, .0001, .0001, .0003, .0003));
            locationList.Add(new LocationPoint("Department Buidling", 27.5728, -99.4345, .0003, .0004, .0001, .0001));
            locationList.Add(new LocationPoint("University of Texas", 30.2850, -97.7335, .0035, .0048, .0063, .0076));
        }

        public int findLogicalLocation(PhysicalLocation userLocation)
        {
            int location = -1;

            for (int i = 0; i <= 5; i++)
            {
                if (isWithinRegion(userLocation, locationList[i]))
                {
                    location = i + 1;
                    break;
                }
            }
            return location;
        }

        private bool isWithinRegion(PhysicalLocation userLocation, LocationPoint region)
        {
            return (region.isInHorizontalBounds(userLocation) && region.isInVerticalBounds(userLocation));
        }
    }
}
