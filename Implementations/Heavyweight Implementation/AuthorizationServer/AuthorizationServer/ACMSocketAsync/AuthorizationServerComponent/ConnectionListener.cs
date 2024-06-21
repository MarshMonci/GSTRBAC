using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ACMSocketAsync {
    public class ConnectionListener {
        ClientManager clientManager;
        TcpClient resourceClient = new TcpClient();

        public ConnectionListener(ClientManager clientManager) {
            this.clientManager = clientManager;
        }

        public async Task ListenForConnections(TcpListener tcpListener, List<TcpClient> clients, IPAddress ipaddr, int port) {

            ipaddr = ipaddr == null ? IPAddress.Any : ipaddr;
            port = (port <= 0 || port > 655355) ? 24000 : port;

            //prints the values to the debug console
            Console.WriteLine(string.Format("Authorization Servere IP Address: {0} - Port: {1}", ipaddr.ToString(), port));

            tcpListener = new TcpListener(ipaddr, port);

            try {
                tcpListener.Start();

                AuthorizationServer.keepRunning = true;
                while (AuthorizationServer.keepRunning) {
                    var resourceServerClient = await tcpListener.AcceptTcpClientAsync();
                    resourceClient = resourceServerClient;

                    clients.Add(resourceClient);


                    Console.WriteLine("Authorization Server connected successfully to resource server IP - {0}",
                        resourceServerClient.Client.RemoteEndPoint.ToString());

                    await clientManager.ManageClient(resourceServerClient);
                }
            }
            catch (Exception excp) {
                Console.WriteLine(excp.ToString());
            }

        }

        public void CloseConnection(TcpListener tcpListener) {
            try {
                if (tcpListener != null) {
                    tcpListener.Stop();
                }

                resourceClient.Close();
            }
            catch (Exception excp) {
                Console.WriteLine(excp.ToString());
            }
        }
    }
}
