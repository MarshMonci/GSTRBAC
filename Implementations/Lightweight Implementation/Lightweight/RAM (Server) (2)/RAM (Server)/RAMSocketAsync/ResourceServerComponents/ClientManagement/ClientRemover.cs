using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace RAMSocketAsync.ResourceServerComponents
{
    public class ClientRemover
    {
        public void RemoveClient(TcpClient paramClient)
        {
            if (paramClient!= null)
            {
                paramClient = null;
                System.Diagnostics.Debug.WriteLine(String.Format("Client removed"));
            }
        }
    }
}
