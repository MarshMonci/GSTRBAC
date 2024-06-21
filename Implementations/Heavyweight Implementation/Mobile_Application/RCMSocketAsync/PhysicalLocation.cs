﻿using System;
using Xamarin.Essentials;

namespace MobileClient {
    public class PhysicalLocation : Location {
        public void UpdateLocation(Location location) {
            Latitude = Math.Round(location.Latitude, 4);
            Longitude = Math.Round(location.Longitude, 4);
            Altitude = location.Altitude;
            Accuracy = location.Accuracy;
        }
    }
}
