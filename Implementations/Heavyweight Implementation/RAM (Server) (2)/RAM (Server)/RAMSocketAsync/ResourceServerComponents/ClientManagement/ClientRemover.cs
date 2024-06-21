using System;
using System.Net.Sockets;

namespace RAMSocketAsync.ResourceServerComponents {
    public class ClientRemover {
        public void RemoveClient(TcpClient paramClient) {
            if (paramClient != null) {
                paramClient = null;
                System.Diagnostics.Debug.WriteLine(String.Format("Client removed"));
            }
        }
    }
}
