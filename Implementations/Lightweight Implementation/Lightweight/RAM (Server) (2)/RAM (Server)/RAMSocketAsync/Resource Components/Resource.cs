using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMSocketAsync.Resource_Components
{
    public interface Resource
    {
        void read();
        Task write();
        Task readWrite();
        Task updateFile(SerializablePackages.Packages.UserInfoPackage userInfo); //UPDATED
    }
}
