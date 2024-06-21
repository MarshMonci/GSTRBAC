﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Data.SqlClient;
using System.Data;

namespace ACMSocketAsync
{
    public class ZoneIdentifier
    {
        public DataRow identifyUserZone(Packages.STZone userZone)
        {
            foreach (DataRow zone in AuthorizationServer.Database.getSTZone().Rows)
            {
                //remove following loop
                //foreach (DataRow location in AuthorizationServer.Database.getLocations().Rows)
                //{
                    //Check if zone[1] == userZone.locationID
                    if (Convert.ToInt32(zone[1]) == userZone.LocationID)
                    {
                        //if (location[1].ToString() == userZone.Location)
                        //{
                            return zone;
                        //}
                    }
                //}
            }

            return null;
        }
    }
}