using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using Xamarin.Essentials;

namespace MobileClient
{
    public class ZoneManager
    {
        LocationTracker locationTracker;
        TimeTracker timeTracker;

        public ZoneManager()
        {
            locationTracker = new LocationTracker();
            timeTracker = new TimeTracker();
        }

        //public void updateLocation(PhysicalLocation updatedLocation)
        //{
        //    locationTracker.setPhysicalLocation(updatedLocation);
        //}

        public int getLocationID()
        {
            return locationTracker.getLocationID(1);
        }

        public Packages.UserInfoPackage fillSTZoneEntry(Packages.UserInfoPackage packageParam, int locationChoice, int timeChoice)
        {
            packageParam.LocationID = Convert.ToInt32(locationTracker.getLocationID(locationChoice));
            packageParam.TimeStamp = timeTracker.getCurrentTimeInterval(timeChoice);

            return packageParam;
        }

        //private Packages.STZone getCurrentZone(string location, DateTime time, Packages packages)
        //{
        //    try
        //    {
        //        currentZone = packages.AssignZone(location, time);
        //        return currentZone;
        //    }
        //    catch (Exception e)
        //    {
        //        //SendText("No STZone matches the user's location and time!");
        //        Debug.WriteLine(e.ToString());
        //        //location set to "Null" in order to act as an identifier that the user is not in a recognized zone
        //        currentZone = new Packages.STZone() { Location = "Null" };
        //        return currentZone;
        //    }
        //}
    }
}
