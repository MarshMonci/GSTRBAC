﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MobileClient
{
    public class GPSClient
    {
        public PhysicalLocation physicalLocation { get; set; }
        CancellationToken cts;

        public GPSClient()
        {
            physicalLocation = new PhysicalLocation();
        }

        public async void updateGPSLocation()
        {
            try
            {
                do
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    cts = new CancellationToken();
                    Location updatedLocation = await Geolocation.GetLocationAsync(request, cts);
                    if (updatedLocation != null)
                    {
                        physicalLocation.UpdateLocation(updatedLocation);
                    }
                } while (true);
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }
    }
}
