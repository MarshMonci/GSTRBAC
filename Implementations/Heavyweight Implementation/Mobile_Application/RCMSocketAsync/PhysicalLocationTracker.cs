namespace MobileClient {
    class PhysicalLocationTracker {
        GPSClient Gps;

        public PhysicalLocationTracker() {
            Gps = new GPSClient();
            Gps.updateGPSLocation();
        }

        public PhysicalLocation getPhysicalLocation() {
            return Gps.physicalLocation;
        }

        public void setPhysicalLocation(PhysicalLocation updatedLocation) {
            Gps.physicalLocation = updatedLocation;
        }
    }
}
