using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace ACMSocketAsync
{
    public class PolicyChecker
    {
        private ZoneIdentifier zoneIdentifier;
        public STZoneChecker ZoneChecker { get; set ;}

        public bool CheckPolicy(Packages.STZone stzone)
        {
            return (ZoneChecker.CheckSTZone(stzone));
        }

        public PolicyChecker()
        {
            zoneIdentifier = new ZoneIdentifier();
            ZoneChecker = new STZoneChecker(zoneIdentifier);
        }
    }
}
