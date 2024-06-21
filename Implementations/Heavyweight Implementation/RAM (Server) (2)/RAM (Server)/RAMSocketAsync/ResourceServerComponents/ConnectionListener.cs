using RAMSocketAsync.ResourceServerComponents.ClientManagement;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RAMSocketAsync.ResourceServerComponents {
    public class ConnectionListener {
        ClientManager clientManager;
        int port;
        IPAddress IPAddr;
        List<TcpClient> listOfClients;
        public TcpListener tcpListener;

        public List<TcpClient> ListOfClients {
            get { return listOfClients; }
        }

        public ConnectionListener(ClientManager clientManager) {
            this.clientManager = clientManager;
            this.listOfClients = new List<TcpClient>();
        }

        public async Task ListenForConnections() {
            IPAddress ipaddrParameter = null;
            int portParameter = 23000;

            if (ipaddrParameter == null) {
                ipaddrParameter = IPAddress.Any;
            }
            if (portParameter <= 0) {
                portParameter = 23000;
            }

            IPAddr = ipaddrParameter;
            port = portParameter;

            //prints the values to the debug console
            Console.WriteLine(string.Format("IP Address: {0} - Port: {1}", IPAddr.ToString(), port));

            tcpListener = new TcpListener(IPAddr, port);

            try {
                tcpListener.Start();

                ResourceServer.keepRunning = true;

                while (ResourceServer.keepRunning) {
                    //returns object of TCP object
                    var returnedByAccept = await tcpListener.AcceptTcpClientAsync();

                    //adds client to list of clients
                    listOfClients.Add(returnedByAccept);
                    Console.WriteLine(string.Format("Client connected successfully, count: {0} - {1}", listOfClients.Count, returnedByAccept.Client.RemoteEndPoint));

                    //when a client gets connected, we want to take care of it
                    try
                    {
                    new Thread(() => clientManager.ManageClient(returnedByAccept)).Start();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    //clientManager.ManageClient(returnedByAccept);
                }
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
