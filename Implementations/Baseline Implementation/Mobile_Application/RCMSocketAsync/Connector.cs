using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;
using SerializablePackages;
using System.Threading;
using System.Timers;

namespace MobileClient
{
    public class Connector
    {
        private System.Timers.Timer timeout;
        private CancellationTokenSource tokenSource;
        private CancellationToken cancelToken;
        bool connecting = false;

        public Connector()
        {
            tokenSource = new CancellationTokenSource();
            cancelToken = tokenSource.Token;
        }

        public async Task<TcpClient> ConnectToServer(TcpClient userClient, IPAddress mServerIPAddress, int mServerPort)
        {

            if (!userClient.Connected)
            {

                try
                {
                    userClient = new TcpClient();
                    Debug.WriteLine("Attempting Connection...");
                    SetConnectionTimeout();
                    connecting = true;
                    userClient.ConnectAsync(mServerIPAddress, mServerPort);
                    while (connecting) {
                    if (userClient.Connected)
                    {
                        timeout.Dispose();
                        Debug.WriteLine(string.Format("Connected to server IP/Port: {0} / {1}",
                        mServerIPAddress, mServerPort));
                        return userClient;
                    }
                    }
                   
                    timeout.Dispose();
                    Debug.WriteLine("Failed to connect to server!");
                    return userClient;
                }
                catch (Exception excp)
                {
                    Debug.WriteLine(excp.ToString());
                    Debug.WriteLine(excp.StackTrace);
                }
            }
            else if (userClient.Connected)
            {
                Debug.WriteLine("User is already connected!");
                return userClient;
            }
            return userClient;
        }

        private void SetConnectionTimeout()
        {
            timeout = new System.Timers.Timer(5000);
            timeout.Start();
            timeout.Elapsed += TimeoutElapsedEvent;
            timeout.AutoReset = false;
            timeout.Enabled = true;
        }

        private void TimeoutElapsedEvent(Object source, ElapsedEventArgs e)
        {
            connecting = false;
        }

    }
}
