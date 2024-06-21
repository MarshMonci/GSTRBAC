using System;

namespace MobileClient {
    public class LocationTracker {
        LogicalLocationFinder LogicalLocFinder;
        PhysicalLocationTracker PhysicalTracker;

        public LocationTracker() {
            LogicalLocFinder = new LogicalLocationFinder();
            PhysicalTracker = new PhysicalLocationTracker();
        }

        public int getLocationID(int locationChoice) {
            int location = -1;
            PhysicalLocation userLocation = PhysicalTracker.getPhysicalLocation();
            try {
                if (userLocation != null) {
                    location = LogicalLocFinder.findLogicalLocation(userLocation);
                }
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
            }

            return location;
        }
    }
}
