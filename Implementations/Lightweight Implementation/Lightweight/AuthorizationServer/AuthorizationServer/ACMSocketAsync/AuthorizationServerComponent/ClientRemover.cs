using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ACMSocketAsync
{
    public class ClientRemover
    {
       public void RemoveClient(TcpClient paramClient)
        {
            if (paramClient != null)
            {
                paramClient.Close(); //UPDATED
                System.Diagnostics.Debug.WriteLine("RAM Server disconnected.");
            }
        }
    }
}
