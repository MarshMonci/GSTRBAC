using RAMSocketAsync.ResourceServerComponents.ClientManagement;
using System;
using System.Net.Sockets;


namespace RAMSocketAsync.ResourceServerComponents {
    public class ResourceServer {
        public static bool keepRunning { get; set; }
        ClientManager clientManager;
        ConnectionListener connectionListener;
        MessageSender messageSender;
        public static ResourceDatabase database { get; set; }

        public ResourceServer() {
            database = new ResourceDatabase();
            messageSender = new MessageSender();
            clientManager = new ClientManager();
            connectionListener = new ConnectionListener(clientManager);
        }

        public void StartServer(string aIPAddress, string portNumber) {
            clientManager.AuthTokenRequester.SetACMIPAddress(aIPAddress);
            clientManager.AuthTokenRequester.SetACMPortNumber(portNumber);
            connectionListener.ListenForConnections();
        }

        public void StopServer() {
            try {
                if (connectionListener.tcpListener != null) {
                    connectionListener.tcpListener.Stop();
                }
                foreach (TcpClient client in connectionListener.ListOfClients) {
                    client.Close();
                }

                connectionListener.ListOfClients.Clear();
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
