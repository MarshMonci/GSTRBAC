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
    public class TextFIleWriteActivity : Activity
    {
        Resource resource;

        public TextFIleWriteActivity(Resource newResource)
        {
            resource = newResource;
        }
        public async Task execute()
        {
            await resource.write(); 
        }
    }
}
