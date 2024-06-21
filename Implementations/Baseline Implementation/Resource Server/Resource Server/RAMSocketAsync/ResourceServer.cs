using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using SerializablePackages;
using System.Diagnostics.Tracing;

namespace RAMSocketAsync
{
    public class ResourceServer
    {
        IPAddress mIP;
        int mPort;
        TcpListener tcpListener;
        //TcpClient RAMclient;
        UserAuthenticator userAuth;
        public AuthorizationRequester authTokenRequester;
        Packages serializer;
        ResourceDatabase database;
        ResourceProvider provider;

        enum enteredInfo
        {
            enumUser = 0,
            enumPass,
            enumLocation,
            enumTime,
            enumFilled
        }

        

        //List created to keep track of clients to RAM server
        List<TcpClient> mClients;

        public bool keepRunning { get; set; }

        public ResourceServer()
        {
            mClients = new List<TcpClient>();
            database = new ResourceDatabase();
            provider = new ResourceProvider(database);
            serializer = new Packages();
            authTokenRequester = new AuthorizationRequester();
        }

        public async Task ListenForConnections(IPAddress ipaddr = null, int port = 23000)
        {
            userAuth = new UserAuthenticator();
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (port <= 0)
            {
                port = 23000;
            }

            mIP = ipaddr;
            mPort = port;

            //prints the values to the debug console
            System.Diagnostics.Debug.WriteLine(string.Format("IP Address: {0} - Port: {1}", mIP.ToString(), mPort));

            tcpListener = new TcpListener(mIP, mPort);

            try
            {
                tcpListener.Start();

                keepRunning = true;

                while (keepRunning)
                {
                    //returns object of TCP object
                    var returnedByAccept = await tcpListener.AcceptTcpClientAsync();

                    //adds client to list of clients
                    mClients.Add(returnedByAccept);
                    System.Diagnostics.Debug.WriteLine(string.Format("Client connected successfully, count: {0} - {1}", mClients.Count,
                        returnedByAccept.Client.RemoteEndPoint));

                    //when a client gets connected, we want to take care of it
                    ManageClient(returnedByAccept);
                }
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }



        private async Task ManageClient(TcpClient paramClient)
        {
            //used to read data from the network stream which is associated with
            //the TCP object passed to the method as a parameter
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = paramClient.GetStream();
                reader = new StreamReader(stream);
                Packages.UserInfoPackage userRequest;

                int accessGranted = 0;
                while (true)
                {
                    System.Diagnostics.Debug.WriteLine("*** Ready to read");
                    userRequest = (Packages.UserInfoPackage)serializer.BinaryDeserializer(paramClient.GetStream());
                    //Initial check to see if credentials are correct
                    if (userAuth.AuthenticateUser(userRequest.Username, userRequest.Password))
                    {
                        System.Diagnostics.Debug.WriteLine("*** Package's User: " + userRequest.Username);
                        userRequest.correctCredentials = true;
                        //RCM is given the OK and the rest of the info can now be sent
                        serializer.BinarySerializer(paramClient.GetStream(), userRequest);
                        break;
                    }
                    else
                    {
                        Packages.ResourcePackage userPackage = new Packages.ResourcePackage { Response = "Incorrect data submitted." };
                        serializer.BinarySerializer(paramClient.GetStream(), userPackage);
                        RemoveClient(paramClient);
                        break;
                    }
                }
                if (userAuth.AuthenticateUser(userRequest.Username, userRequest.Password))
                {
                    while (keepRunning)
                    {
                        //Main bulk of the user request data is now received
                        userRequest = (Packages.UserInfoPackage)serializer.BinaryDeserializer(paramClient.GetStream());
                        await authTokenRequester.ConnectToACMServer();

                        Packages.STZone userZone = serializer.AssignZone(userRequest.LocationID, userRequest.TimeStamp);


                        await authTokenRequester.sendSTZoneData(userZone);
                        accessGranted = authTokenRequester.receiveToken();
                        //verifies the user's zone
                        if (userZone.Location == "Null")
                        {
                            Packages.ResourcePackage userPackage = new Packages.ResourcePackage()
                            {
                                Response = "Outsize of STZone! Check your time and location" +
                                " of request."
                            };
                            serializer.BinarySerializer(paramClient.GetStream(), userPackage);
                        }
                        else if (accessGranted == 1)
                        {
                            //change name to requestedResource
                            Packages.ResourcePackage userPackage = new Packages.ResourcePackage() { Response = "Token has been granted by the ACM." };


                            string filepath = provider.getResource(userRequest.ResourceID);

                            if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.Read))
                            {
                                provider.ReadActivity(filepath, paramClient.GetStream(), userPackage, serializer);
                            }
                            else if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.ReadWrite))
                            {
                                provider.WriteActivity(filepath, paramClient.GetStream(), userPackage, serializer);
                            }
                            else if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.ReadWrite))
                            {
                                provider.ReadWriteActivity(filepath, paramClient.GetStream(), userPackage, serializer);
                            }
                        }
                        else if (accessGranted == 0)
                        {
                            Packages.ResourcePackage userPackage = new Packages.ResourcePackage() { Response = "User not authorized to request resource." };
                            serializer.BinarySerializer(paramClient.GetStream(), userPackage);
                        }
                    }
                }
            }
            catch (Exception excp)
            {
                RemoveClient(paramClient);
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

       

        public async Task SendMessage(TcpClient client, string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }
            try
            {
                //changes message into byte format
                byte[] buffMessage = Encoding.ASCII.GetBytes(message);
                client.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

        public void StopServer()
        {
            try
            {
                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }
                //goes through list of clients and closes
                //them sequentially
                foreach (TcpClient c in mClients)
                {
                    c.Close();
                }
                //after sockets are closed, the array is cleared
                //not entirely necessary to call this but it can be just to be safe
                mClients.Clear();
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

        //Task here is equivalent to void
        //best practices recommend the use of task rather than void
        private async Task ReadDataAsync(TcpClient clientParam)
        {
            try
            {
                //used to read data from network stream
                StreamReader clientStreamReader = new StreamReader(authTokenRequester.RAMClient.GetStream(), Encoding.Default, true, 512, true);
                //used to store data sent by server
                char[] buff = new char[64];
                //used to store number of characters received
                int readByteCount = 0;

                while (true)
                {
                    readByteCount = await clientStreamReader.ReadAsync(buff, 0, buff.Length);

                    if (readByteCount <= 0)
                    {
                        Console.WriteLine("Disconnected from server.");
                        authTokenRequester.RAMClient.Close();
                        break;
                    }
                    Console.WriteLine(string.Format("Received bytes: {0} - Message: {1}", readByteCount,
                       new string(buff)));
                    if (new string(buff).Trim('\0') == "token")
                    {
                        await SendMessage(clientParam, "ACM has granted a " + new string(buff).Trim('\0') + ".");
                        Array.Clear(buff, 0, buff.Length);
                        break;
                    }
                    else { Array.Clear(buff, 0, buff.Length); }
                }

            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

        private void RemoveClient(TcpClient paramClient)
        {
            if (mClients.Contains(paramClient))
            {
                mClients.Remove(paramClient);
                System.Diagnostics.Debug.WriteLine(String.Format("Client removed, count: {0}", mClients.Count));
            }
        }
    }
}
