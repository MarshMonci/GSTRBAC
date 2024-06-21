using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Net.Sockets;
using System.IO;

namespace RAMSocketAsync.Resource_Components
{
    public class TextFIleReadWriteActivity : Activity
    {
        Resource resource;

        public TextFIleReadWriteActivity(Resource newResource)
        {
            resource = newResource;
        }
        public async Task execute()
        {
            resource.readWrite();
        }
    }
}
