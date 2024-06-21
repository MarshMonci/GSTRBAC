using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SerializablePackages;
using System.Net.Sockets;
using RAMSocketAsync.ResourceServerComponents;

namespace RAMSocketAsync.Resource_Components
{
    public class ResourceProvider
    {
        ResourceDatabase database;
        public ResourceProvider()
        {
        }

        //UPDATE
        public string getResource(int request)
        {
            return ResourceServer.database.getFile(request);
        }    
    }
}
