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
    public class TextFIleReadActivity : Activity
    {
        Resource resource;

        public TextFIleReadActivity(Resource newResource)
        {
            resource = newResource;
        }
        public Task execute()
        {
           resource.read();
           return Task.FromResult(0);
        }
    }
}
