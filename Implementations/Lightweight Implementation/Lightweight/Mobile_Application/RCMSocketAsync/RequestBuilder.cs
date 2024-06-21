using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using SerializablePackages;
using Xamarin.Forms;
using System.Net;
using System.Diagnostics;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System.Net.NetworkInformation;

namespace MobileClient {
    public class RequestBuilder
    {
        IPAddress[] IPList;
        private IPAddress mServerIPAddress;
        private int mServerPort;
        //private TcpClient userClient;
        public Connector userConnector;
        public CredentialVerifier credentialVerifier;
        public Stopwatch requestTimer;
        public Stopwatch encryptionTimer;
        public Stopwatch totalTimer;
        public Stopwatch decryptionTimer;
        static int requestNumber;
        public TcpClient UserClient { get; set; }
        public Packages.UserInfoPackage userInfo;
        ZoneManager STZManager;
        Packages serializer;
        Label appConsole;
        PdfDocument readerPdf;
        public RequestBuilder(ZoneManager zoneManager)
        {
            UserClient = new TcpClient();
            userInfo = new Packages.UserInfoPackage();
            STZManager = zoneManager;
            serializer = new Packages();
            userConnector = new Connector();
            credentialVerifier = new CredentialVerifier();
            requestTimer = new Stopwatch();
            encryptionTimer = new Stopwatch();
            totalTimer = new Stopwatch();
            decryptionTimer = new Stopwatch();
            requestNumber = 0;
            IPAddress[] newArray = new IPAddress[3];
            newArray[2] = IPAddress.Parse("20.88.210.119");
            newArray[1] = IPAddress.Parse("137.135.124.207");
            newArray[0] = IPAddress.Parse("40.91.96.162");

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

        public bool SetServerIPAddress(string IPAddressServer)
        {
            //IPAddress ipaddr = null;

            ////checks to see if IP address supplied is valid
            ////if it isn't then the program will
            //if (!IPAddress.TryParse(IPAddressACM, out ipaddr))
            //{
            //    Console.WriteLine("Invalid server IP supplied.");
            //    return false;
            //}

            //ACMIP = ipaddr;
            //return true;
            Debug.WriteLine("Checking available Resource server hosts...");


            int RTT_MinIndex = 0;
            long RTT_Min = 5000;
            IPStatus Min_Status = new IPStatus();

            Ping prePing = new Ping();
            PingReply preReply;

            try {
                preReply = prePing.Send(IPList[0], 3000);

                if (preReply.Status == IPStatus.Success) {
                    Debug.WriteLine("Ping to " + IPList[0].ToString() + "[" + preReply.Address.ToString() + "]" + "Response delay = " +
                        preReply.RoundtripTime.ToString() + " ms");
                    RTT_Min = preReply.RoundtripTime;
                }
                else {
                    Debug.WriteLine("Error code: " + preReply.Status.ToString());
                    Debug.WriteLine("Could not ping " + IPList[0].ToString() + "...");
                    RTT_Min = 6000;
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
            prePing.Dispose();

            IPStatus currentStatus = new IPStatus();
            for (int i = 1; i < IPList.Length; i++) {
                long RTT = 10000;

                //Find RTT
                Ping p = new Ping();
                PingReply reply;
                for (int j = 0; j < 2; j++) {
                    reply = p.Send(IPList[i], 3000);

                    if (reply.Status == IPStatus.Success) {
                        Debug.WriteLine("Ping to " + IPList[i].ToString() + "[" + reply.Address.ToString() + "]" + "Response delay = " +
                            reply.RoundtripTime.ToString() + " ms");
                        if (reply.RoundtripTime < RTT) {
                            RTT = reply.RoundtripTime;
                            currentStatus = reply.Status;
                        }
                    }
                    else {
                        Debug.WriteLine("Error code: " + reply.Status.ToString());
                        Debug.WriteLine("Could not ping " + IPList[i].ToString() + "...");
                    }
                    //try
                    //{
                    //    Console.WriteLine("Ping to " + IPList[i].ToString() + "[" + reply.Address.ToString() + "]" + "Response delay = " +
                    //        reply.RoundtripTime.ToString() + " ms");
                    //    RTT = reply.RoundtripTime;
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e.ToString());
                    //}
                }
                p.Dispose();

                if (RTT < RTT_Min) {
                    RTT_Min = RTT;
                    RTT_MinIndex = i;
                    Min_Status = currentStatus;
                }
            }

            if (Min_Status != IPStatus.Success) {
                Debug.WriteLine("Warning! Selected IP returns error code: " + Min_Status.ToString());
            }
            Debug.WriteLine("Server being connected to is \"" + IPList[RTT_MinIndex].ToString() + "\"...");
            mServerIPAddress = IPList[RTT_MinIndex];
            //Console.WriteLine("Server being connected to is \"" + "192.168.86.176" + "\"...");
            //ACMIP = IPAddress.Parse("23.98.141.50");
            return true;
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

        //create Connection class
        public async Task<ECDHUser> ConnectToServer(ECDHUser userCryptoInfo)
        {
            var result = await userConnector.ConnectToServer(UserClient, mServerIPAddress, mServerPort, userCryptoInfo);
            UserClient = result.Item1;
            return result.Item2;
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

        public void SetPdfReader(PdfDocument pdf)
        {
            readerPdf = pdf;
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

        public void SendPdf(PdfLoadedDocument loadedDocument)
        {
            try
            {
                PdfDocumentBase.Merge(readerPdf, loadedDocument);
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

        public bool CheckOngoingAuthorization(ECDHUser crypto)
        {
            try
            {
                if (UserClient.Connected)
                {
                    totalTimer.Restart();
                    totalTimer.Start();

                    encryptionTimer.Restart();
                    encryptionTimer.Start();
                    AESEncryptor encryptor = new AESEncryptor(crypto.GetKey());
                    Packages.EncryptedPackage encrypted = encryptor.Encrypt(userInfo);
                    encryptionTimer.Stop();

                    requestTimer.Restart();
                    requestTimer.Start();
                    serializer.BinarySerializer(UserClient.GetStream(), encrypted);                    
                    encrypted = (Packages.EncryptedPackage)serializer.BinaryDeserializer(UserClient.GetStream());
                    requestTimer.Stop();

                    decryptionTimer.Restart();
                    decryptionTimer.Start();
                    AESDecryptor decryptor = new AESDecryptor(crypto.GetKey());
                    var RAMPackage = (Packages.ResourcePackage)decryptor.DecryptAndRead(encrypted);
                    decryptionTimer.Stop();
                    totalTimer.Stop();
                    //var RAMPackage = (Packages.ResourcePackage)serializer.BinaryDeserializer(UserClient.GetStream());

                    string totalTimeTaken = string.Format("{0}", totalTimer.Elapsed.Seconds * 1000 + totalTimer.Elapsed.Milliseconds);
                    string encryptionTime = string.Format("{0}", encryptionTimer.Elapsed.Seconds * 1000 + encryptionTimer.Elapsed.Milliseconds);
                    string decryptionTime = string.Format("{0}", decryptionTimer.Elapsed.Seconds * 1000 + decryptionTimer.Elapsed.Milliseconds);
                    string requestTime = string.Format("{0}", requestTimer.Elapsed.Seconds * 1000 + requestTimer.Elapsed.Milliseconds);

                    using(StreamWriter streamWriter = File.AppendText(Path.Combine("/storage/emulated/0/Android/data/tamiu.gstrbac.mobile_app/files/", "ConnectionLog.txt"))) {
                        requestNumber += 1;
                        streamWriter.WriteLine("Request #" + Convert.ToString(requestNumber) + " Total Request Duration: " + totalTimeTaken);
                        streamWriter.WriteLine("Request #" + Convert.ToString(requestNumber) + " Time to encrypt: " + encryptionTime);
                        streamWriter.WriteLine("Request #" + Convert.ToString(requestNumber) + " Time to decrypt: " + decryptionTime);
                        streamWriter.WriteLine("Request #" + Convert.ToString(requestNumber) + " Time to request: " + requestTime);
                        streamWriter.Close();
                    }

                    Console.WriteLine("RAM Response: " + RAMPackage.Response);

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

        public async Task CredentialVerification(string username, string password, NetworkStream userStream, ECDHUser crpytoInfo)
        {
            userInfo = credentialVerifier.VerifyCredentials(username, password, userStream, userInfo, crpytoInfo);
        }

        public async Task requestResourcesFromServer(NetworkStream userStream, string systemFilepath, ECDHUser ecdhUser)
        {
            AESEncryptor encryptor = new AESEncryptor(ecdhUser.GetKey());
            Packages.EncryptedPackage encrypted = encryptor.Encrypt(userInfo);
            serializer.BinarySerializer(userStream, encrypted);
            awaitPackage(userStream, systemFilepath, ecdhUser);
        }

        public bool CheckIfValidRequest()
        {
            if(STZManager.getLocationID() != -1) { return true; }
            else { return false; }
        }

        public async Task awaitPackage(NetworkStream stream, string systemFilepath, ECDHUser ecdhUser)
        {
            Packages.ResourcePackage RAMPackage;

            try
            {
                AESDecryptor decryptor = new AESDecryptor(ecdhUser.GetKey());
                Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.BinaryDeserializer(stream);
                RAMPackage = (Packages.ResourcePackage)decryptor.DecryptAndRead(encrypted);
                //RAMPackage = (Packages.ResourcePackage)serializer.BinaryDeserializer(stream);

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
                        PdfLoadedDocument loadedDocument = new PdfLoadedDocument(RAMPackage.ResourceFile);
                        SendPdf(loadedDocument);
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
    }
}
