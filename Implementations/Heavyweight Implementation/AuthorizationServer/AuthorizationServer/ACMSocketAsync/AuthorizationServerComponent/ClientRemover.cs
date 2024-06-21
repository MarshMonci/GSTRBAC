using System.Net.Sockets;

namespace ACMSocketAsync {
    public class ClientRemover {
        public void RemoveClient(TcpClient paramClient) {
            if (paramClient != null) {
                paramClient.Close(); //UPDATED
                System.Diagnostics.Debug.WriteLine("RAM Server disconnected.");
            }
        }
    }
}
