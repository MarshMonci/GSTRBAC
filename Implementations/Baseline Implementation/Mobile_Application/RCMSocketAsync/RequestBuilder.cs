using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using MobileClient;
using System.Xml.Serialization;
using SerializablePackages;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Net;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Plugin.Connectivity;

namespace MobileClient
{
    public class RequestBuilder
    {
        private IPAddress mServerIPAddress;
        private int mServerPort;
        //private TcpClient userClient;
        public Connector userConnector;
        public CredentialVerifier credentialVerifier;
        public Stopwatch requestTimer;
        public TcpClient UserClient { get; set; }
        public Packages.UserInfoPackage userInfo;
        static int requestNumber;
        ZoneManager STZManager;
        Packages serializer;
        IPAddress[] IPList;


        Label appConsole;
        public RequestBuilder(ZoneManager zoneManager)
        {
            UserClient = new TcpClient();
            userInfo = new Packages.UserInfoPackage();
            STZManager = zoneManager;
            serializer = new Packages();
            userConnector = new Connector();
            credentialVerifier = new CredentialVerifier();
            requestTimer = new Stopwatch();
            requestNumber = 0;

            IPAddress[] newArray = new IPAddress[3];
            newArray[2] = IPAddress.Parse("20.88.210.119");
            newArray[1] = IPAddress.Parse("40.117.91.100");
            newArray[0] = IPAddress.Parse("13.77.148.227");

            IPList = newArray;
        }

        public EventHandler<WriteToFileEventArgs> RaiseWriteToFileEvent;

        protected virtual async Task OnRaiseWriteToFileEvent(WriteToFileEventArgs writeFile)
        {
            EventHandler<WriteToFileEventArgs> handler = RaiseWriteToFileEvent;

            if (handler != null)
            {
                handler(this, writeFile);
            }
        }

        public IPAddress ServerIPAddress
        {
            get
            {
                return mServerIPAddress;
            }
        }

        public int ServerPort
        {
            get
            {
                return mServerPort;
            }
        }

        public async Task SetServerIPAddress()
        {
            IPAddress ipaddr = null;

            Debug.WriteLine("Checking available server connectivity...");
            
            
            int RTT_MinIndex = 0;
            long RTT_Min = 5000;
            IPStatus Min_Status = new IPStatus();
            bool anyServerPingable = false;

            Ping prePing = new Ping();
            PingReply preReply;
            PingOptions options = new PingOptions(200, true);
            byte[] buffer = new byte[128];

            preReply = prePing.Send(IPList[0], 3000, buffer, options);
            bool connectivity = await CrossConnectivity.Current.IsReachable(IPList[0].ToString());

            try
            {
                if (preReply.Status == IPStatus.Success)
                {
                    Debug.WriteLine("Ping to " + IPList[0].ToString() + "[" + preReply.Address.ToString() + "]" + "Response delay = " +
                        preReply.RoundtripTime.ToString() + " ms");
                    RTT_Min = preReply.RoundtripTime;
                    anyServerPingable = true;
                }
                else if (connectivity)
                {
                    Debug.WriteLine("Error code: " + preReply.Status.ToString());
                    Debug.WriteLine("Ping could not be established, but connection can be made to a server.");
                    RTT_Min = 6000;
                }
                else
                {
                    Debug.WriteLine("Error code: " + preReply.Status.ToString());
                    Debug.WriteLine("Could not ping " + IPList[0].ToString() + "...");
                    RTT_Min = 6000;
                }
                Min_Status = preReply.Status;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            prePing.Dispose();

            IPStatus currentStatus = new IPStatus();
            for (int i = 1; i < IPList.Length; i++)
            {
                long RTT = 10000;

                //Find RTT
                Ping p = new Ping();
                PingReply reply;
                for (int j = 0; j < 2; j++)
                {
                    reply = p.Send(IPList[i], 3000, buffer, options);

                    if (reply.Status == IPStatus.Success)
                    {
                        Debug.WriteLine("Ping to " + IPList[i].ToString() + "[" + reply.Address.ToString() + "]" + "Response delay = " +
                            reply.RoundtripTime.ToString() + " ms");
                        anyServerPingable = true;
                        if (reply.RoundtripTime < RTT)
                        {
                            RTT = reply.RoundtripTime;
                            currentStatus = reply.Status;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Error code: " + reply.Status.ToString());
                        Debug.WriteLine("Could not ping " + IPList[i].ToString() + "...");
                    }
                }
                p.Dispose();

                if (RTT < RTT_Min)
                {
                    RTT_Min = RTT;
                    RTT_MinIndex = i;
                    Min_Status = currentStatus;
                }
            }

            if (Min_Status != IPStatus.Success && !anyServerPingable)
            {
                Debug.WriteLine("Warning! Selected IP returns error code: " + Min_Status.ToString());
            }

            if (!anyServerPingable)
            {
                Debug.WriteLine("Server being connected to is \"" + IPList[0].ToString() + "\"...");
                mServerIPAddress = IPAddress.Parse(IPList[0].ToString());
            }
            else
            {
                Debug.WriteLine("Server being connected to is \"" + IPList[RTT_MinIndex].ToString() + "\"...");
                mServerIPAddress = IPList[RTT_MinIndex];
            }
        }

        public bool SetPortNumber(string ServerPort)
        {
            int portNumber = 0;

            
            if (!int.TryParse(ServerPort.Trim(), out portNumber))
            {
                return false;
            }

            if (portNumber <= 0 || portNumber > 65535)
            {
                return false;
            }

            mServerPort = portNumber;
            return true;
        }

        public async Task ConnectToServer()
        {
            UserClient = await userConnector.ConnectToServer(UserClient, mServerIPAddress, mServerPort);
        }

        public void CloseAndDisconnect()
        {
            try
            {
                if (UserClient != null)
                {
                    if (UserClient.Connected)
                    {
                        UserClient.Close();
                        userInfo = new Packages.UserInfoPackage();
                        Debug.WriteLine("Client disconnected from server!");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void SetTextBox(Label textBox)
        {
            appConsole = textBox;
        }

        public void SendText(string newText)
        {
            try
            {
                appConsole.Text = appConsole.Text + newText + Environment.NewLine;
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }

        }

        public async Task ReceiveFormRequest(int activity, int resource, int role, int locationChoice, int timeIndexChoice)
        {
            //consider refactoring switch statements to separate classes
            int activityID;
            switch(activity)
            {
                case 0:
                    activityID = (int)Packages.Actions.Read;
                    userInfo.requestedActivity = new Packages.Activity(activityID);
                    break;
                case 1:
                    activityID = (int)Packages.Actions.ReadWrite;
                    userInfo.requestedActivity = new Packages.Activity(activityID);
                    break;
                case 2:
                    activityID = (int)Packages.Actions.ReadWrite;
                    userInfo.requestedActivity = new Packages.Activity(activityID);
                    break;
            }
            userInfo.ResourceID = resource + 1;
            userInfo.RoleID = role + 1;
            userInfo = STZManager.fillSTZoneEntry(userInfo, locationChoice, timeIndexChoice);
        }

        public async Task CredentialVerification(string username, string password, NetworkStream userStream)
        {
            userInfo = credentialVerifier.CredentialVerification(username, password, userStream, userInfo);
        }

        public async Task requestResourcesFromServer(NetworkStream userStream, string systemFilepath)
        {
            requestTimer.Restart();
            requestTimer.Start();
            serializer.XMLSerializer(userStream, userInfo, typeof(Packages.UserInfoPackage));
            awaitPackage(userStream, systemFilepath);
        }

        public bool CheckIfValidRequest()
        {
            if(STZManager.getLocationID() != -1) { return true; }
            else { return false; }
        }

        public async Task awaitPackage(NetworkStream stream, string systemFilepath)
        {
            Packages.ResourcePackage RAMPackage;

            try
            {
                RAMPackage = (Packages.ResourcePackage)serializer.XMLDeserializer(stream, typeof(Packages.ResourcePackage));
                requestTimer.Stop();
                string elapsedTime = String.Format("{0}", requestTimer.Elapsed.Seconds * 1000 + requestTimer.Elapsed.Milliseconds);
                using (StreamWriter sWriter = File.AppendText(Path.Combine("/storage/emulated/0/Android/data/tamiu.gstrbac.mobile_app/files/", "ConnectionLog.txt")))
                {
                    requestNumber += 1;
                    sWriter.WriteLine("Request #" + Convert.ToString(requestNumber) + " Resource request duration: " + elapsedTime);
                    Console.WriteLine("Request #" + Convert.ToString(requestNumber) + " Resource request duration: " + elapsedTime);
                    sWriter.Close();
                }

                if (RAMPackage.Response == "User not authorized to request resource.")
                {
                    SendText(RAMPackage.Response);
                    return;
                }
                else if (RAMPackage.Response == "Outsize of STZone! Check your time and location of request.")
                {
                    SendText(RAMPackage.Response);
                    return;
                }

                string filepath;
                if (userInfo.ResourceID == 3)
                {
                    filepath = Path.Combine(systemFilepath, "Response.pdf");
                }
                else
                {
                    filepath = Path.Combine(systemFilepath, "Response.txt");
                }

                if (userInfo.requestedActivity.ActivityID == (int)Packages.Actions.Read)
                {
                    if (userInfo.ResourceID == 3)
                    {
                        //File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                        
                    }
                    else
                    {
                        File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                        string[] response = File.ReadAllLines(filepath);
                        for (int i = 0; i < response.Length; i++)
                        {
                            SendText(response[i]);
                        }
                        File.Delete(filepath);
                    }
                }
                else if (userInfo.requestedActivity.ActivityID == (int)Packages.Actions.ReadWrite)
                {
                    File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                    string[] response = File.ReadAllLines(filepath);
                    for (int i = 0; i < response.Length; i++)
                    {
                        SendText(response[i]);
                    }
                    await OnRaiseWriteToFileEvent(new WriteToFileEventArgs(filepath));
                }
                else if (userInfo.requestedActivity.ActivityID == (int)Packages.Actions.ReadWrite)
                {
                    File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                    await OnRaiseWriteToFileEvent(new WriteToFileEventArgs(filepath));
                }
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

        public bool CheckOngoingAuthorization()
        {
            try
            {
                if (UserClient.Connected)
                {
                    requestTimer.Restart();
                    requestTimer.Start();
                    serializer.XMLSerializer(UserClient.GetStream(), userInfo, typeof(Packages.UserInfoPackage));
                    var RAMPackage = (Packages.ResourcePackage)serializer.XMLDeserializer(UserClient.GetStream(), typeof(Packages.ResourcePackage));
                    requestTimer.Stop();
                    string elapsedTime = String.Format("{0}", requestTimer.Elapsed.Seconds * 1000 + requestTimer.Elapsed.Milliseconds);
                    using (StreamWriter sWriter = File.AppendText(Path.Combine("/storage/emulated/0/Android/data/tamiu.gstrbac.mobile_app/files/", "ConnectionLog.txt")))
                    {
                        requestNumber += 1;
                        sWriter.WriteLine("Request #" + Convert.ToString(requestNumber) + " Resource request duration: " + elapsedTime);
                        Console.WriteLine("Request #" + Convert.ToString(requestNumber) + " Resource request duration: " + elapsedTime);
                        sWriter.Close();
                    }

                    if (RAMPackage.Response == "User not authorized to request resource.")
                    {
                        SendText(RAMPackage.Response);
                        return false;
                    }
                    else if (RAMPackage.Response == "Outsize of STZone! Check your time and location of request.")
                    {
                        SendText(RAMPackage.Response);
                        return false;
                    }
                    else { return true; }
                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
            return false;
        }
    }
}
