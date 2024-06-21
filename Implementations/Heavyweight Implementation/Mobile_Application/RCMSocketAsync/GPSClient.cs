using System;
using System.Diagnostics;
using System.Threading;
using Xamarin.Essentials;

namespace MobileClient {
    public class GPSClient {
        public PhysicalLocation physicalLocation { get; set; }
        CancellationToken cts;

        public GPSClient() {
            physicalLocation = new PhysicalLocation();
        }

        //public string getCurrentLocation()
        //{
        //    //test value
        //    depreciatedLocation = "Laredo";

        //    return depreciatedLocation;
        //}

        public async void updateGPSLocation() {
            try {
                do {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    cts = new CancellationToken();
                    Location updatedLocation = await Geolocation.GetLocationAsync(request, cts);
                    if (updatedLocation != null) {
                        physicalLocation.UpdateLocation(updatedLocation);
                    }
                } while (true);
            }
            catch (Exception excp) {
                Debug.WriteLine(excp.ToString());
            }
        }
    }
}
