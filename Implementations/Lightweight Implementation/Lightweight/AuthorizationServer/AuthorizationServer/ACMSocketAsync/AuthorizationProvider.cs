using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Data.SqlClient;
using System.Data;

namespace ACMSocketAsync
{
    public class AuthorizationProvider
    {
        public bool checkAccess(Packages.Resource r, Activity a, Packages.STZone stzone)
        {
            return (AuthorizationServer.Checker.CheckPolicy(stzone));
        }
    }

    public class Activity
    {
        public bool exists;

        public Activity()
        {
            exists = true;
        }
    }
}
