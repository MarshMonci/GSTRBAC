using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MobileClient {
    public class Connector {
        public async Task<(TcpClient, RSAUser)> ConnectToServer(TcpClient userClient, IPAddress mServerIPAddress, int mServerPort, RSAUser userCryptoInfo) {
            //if (userClient == null)
            //{
            //    userClient = new TcpClient();
            //}

            if (!userClient.Connected) {
                try {
                    userClient = new TcpClient();
                    Debug.WriteLine("Attempting Connection...");
                    await userClient.ConnectAsync(mServerIPAddress, mServerPort);
                    if (userClient.Connected) {
                        Debug.WriteLine(string.Format("Connected to server IP/Port: {0} / {1}",
                        mServerIPAddress, mServerPort));

                        ServerHandshake handshake = new ServerHandshake();
                        try {
                            await handshake.Initialize(userCryptoInfo, userClient.GetStream());
                        }
                        catch (Exception e) {
                            Debug.WriteLine(e);
                        }

                        return (userClient, userCryptoInfo);
                    }
                    else {
                        Debug.WriteLine("Failed to connect to server!");
                        return (userClient, userCryptoInfo);
                    }
                }
                catch (Exception excp) {
                    Debug.WriteLine(excp.ToString());
                    Debug.WriteLine(excp.StackTrace);
                }
            }
            else if (userClient.Connected) {
                Debug.WriteLine("User is already connected!");
                return (userClient, userCryptoInfo);
            }
            return (userClient, userCryptoInfo);
        }
    }
}
