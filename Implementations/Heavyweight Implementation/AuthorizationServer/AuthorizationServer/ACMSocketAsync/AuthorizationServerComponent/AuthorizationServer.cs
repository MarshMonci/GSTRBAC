using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ACMSocketAsync {
    public class AuthorizationServer {
        List<TcpClient> clientList { get; set; }
        TcpListener tcpListener;
        ClientManager clientManager;
        ConnectionListener connectionListener;
        AuthorizationProvider authorizationProvider;
        static public PolicyChecker Checker { get; set; }
        static public AuthorizationDatabase Database { get; set; }
        static public bool keepRunning { get; set; }
        public AuthorizationServer() {
            Checker = new PolicyChecker();
            Database = new AuthorizationDatabase();
            authorizationProvider = new AuthorizationProvider();
            clientManager = new ClientManager(authorizationProvider);
            connectionListener = new ConnectionListener(clientManager);
            clientList = new List<TcpClient>();
        }

        public async Task StartServer(IPAddress ipaddr = null, int port = 24000) {
            await connectionListener.ListenForConnections(tcpListener, clientList, ipaddr, port);
        }
        public void StopServer() {
            connectionListener.CloseConnection(tcpListener);
        }
    }
}
