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
    public class TextFileActivity : Activity
    {
        public Task execute()
        {
            return Task.FromResult(1);
        }
    }
}
