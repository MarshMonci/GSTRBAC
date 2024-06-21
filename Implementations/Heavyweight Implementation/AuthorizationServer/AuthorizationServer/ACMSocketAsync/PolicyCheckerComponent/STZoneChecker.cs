﻿using SerializablePackages;
using System.Data;

namespace ACMSocketAsync {
    public class STZoneChecker {
        ZoneIdentifier zoneIdentifier = null;
        TimeChecker timeChecker = null;
        public STZoneChecker(ZoneIdentifier zoneIdentifier) {
            this.zoneIdentifier = zoneIdentifier;
            timeChecker = new TimeChecker();
        }
        public bool CheckSTZone(Packages.STZone userZone) {
            DataRow userSTZone = zoneIdentifier.identifyUserZone(userZone);

            if (userSTZone != null) {
                foreach (DataRow interval in AuthorizationServer.Database.getTimeIntervals().Rows) {
                    if (userSTZone[2].ToString() == interval[0].ToString()) {
                        return (timeChecker.CheckTime(interval, userZone));
                    }
                }
            }
            return false;
        }


    }
}