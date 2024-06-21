namespace MobileClient {
    class LocationPoint {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string locationName { get; set; }
        public double northOffset { get; set; }
        public double southOffset { get; set; }
        public double westOffset { get; set; }
        public double eastOffset { get; set; }


        public LocationPoint(string name, double latitude, double longitude, double north,
            double south, double east, double west) {
            this.Latitude = latitude;
            this.Longitude = longitude;
            northOffset = Latitude + north;
            southOffset = Latitude - south;
            westOffset = Longitude - west;
            eastOffset = Longitude + east;
        }

        public bool isInHorizontalBounds(PhysicalLocation userLocation) {
            if (userLocation.Longitude <= eastOffset && userLocation.Longitude >= westOffset) {
                return true;
            }
            else {
                return false;
            }
        }

        public bool isInVerticalBounds(PhysicalLocation userLocation) {
            if (userLocation.Latitude <= northOffset && userLocation.Latitude >= southOffset) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
