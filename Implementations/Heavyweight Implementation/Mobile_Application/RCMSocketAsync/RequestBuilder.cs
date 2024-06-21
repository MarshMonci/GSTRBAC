using SerializablePackages;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileClient {
    public class RequestBuilder {
        private IPAddress mServerIPAddress;
        private int mServerPort;
        //private TcpClient userClient;
        public Connector userConnector;
        public CredentialVerifier credentialVerifier;
        public TcpClient UserClient { get; set; }
        public Packages.UserInfoPackage userInfo;
        public string androidFilepath;

        Stopwatch encryptTimer = new Stopwatch();
        Stopwatch decryptTimer = new Stopwatch();
        Stopwatch timer = new Stopwatch();
        int count = 1;

        ZoneManager STZManager;
        Packages serializer;
        Label appConsole;
        PdfDocument readerPdf;
        IPAddress[] IPList;
        public RequestBuilder(ZoneManager zoneManager) {
            UserClient = new TcpClient();
            userInfo = new Packages.UserInfoPackage();
            STZManager = zoneManager;
            serializer = new Packages();
            userConnector = new Connector();
            credentialVerifier = new CredentialVerifier();

            IPAddress[] newArray = new IPAddress[3];
            newArray[0] = IPAddress.Parse("20.88.210.119");
            newArray[1] = IPAddress.Parse("13.77.148.227");
            newArray[2] = IPAddress.Parse("40.117.91.100");

            IPList = newArray;
        }

        public EventHandler<WriteToFileEventArgs> RaiseWriteToFileEvent;

        protected virtual async Task OnRaiseWriteToFileEvent(WriteToFileEventArgs writeFile) {
            EventHandler<WriteToFileEventArgs> handler = RaiseWriteToFileEvent;

            if (handler != null) {
                handler(this, writeFile);
            }
        }

        public IPAddress ServerIPAddress {
            get {
                return mServerIPAddress;
            }
        }

        public int ServerPort {
            get {
                return mServerPort;
            }
        }
        public bool setAndroidDirectory(string filepath)
        {
            androidFilepath = filepath;
            Debug.WriteLine("Using filepath: " + Path.Combine(filepath, "ConnectionLog.txt"));
            return true;
        }
        public bool SetServerIPAddress(string IPAddressServer) 
        {
            Console.WriteLine("Checking available Authorization server hosts...");


            int RTT_MinIndex = 0;
            long RTT_Min = 5000;
            IPStatus Min_Status = new IPStatus();

            Ping prePing = new Ping();
            PingReply preReply;

            try
            {
                preReply = prePing.Send(IPList[0], 3000);

                if (preReply.Status == IPStatus.Success)
                {
                    Console.WriteLine("Ping to " + IPList[0].ToString() + "[" + preReply.Address.ToString() + "]" + "Response delay = " +
                        preReply.RoundtripTime.ToString() + " ms");
                    RTT_Min = preReply.RoundtripTime;
                }
                else
                {
                    Console.WriteLine("Error code: " + preReply.Status.ToString());
                    Console.WriteLine("Could not ping " + IPList[0].ToString() + "...");
                    RTT_Min = 6000;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
                    reply = p.Send(IPList[i], 3000);

                    if (reply.Status == IPStatus.Success)
                    {
                        Console.WriteLine("Ping to " + IPList[i].ToString() + "[" + reply.Address.ToString() + "]" + "Response delay = " +
                            reply.RoundtripTime.ToString() + " ms");
                        if (reply.RoundtripTime < RTT)
                        {
                            RTT = reply.RoundtripTime;
                            currentStatus = reply.Status;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error code: " + reply.Status.ToString());
                        Console.WriteLine("Could not ping " + IPList[i].ToString() + "...");
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

            if (Min_Status != IPStatus.Success)
            {
                Console.WriteLine("Warning! Selected IP returns error code: " + Min_Status.ToString());
            }
            Console.WriteLine("Server being connected to is \"" + IPList[RTT_MinIndex].ToString() + "\"...");
            mServerIPAddress = IPList[RTT_MinIndex];
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
        public async Task<RSAUser> ConnectToServer(RSAUser userCryptoInfo) {
            var result = await userConnector.ConnectToServer(UserClient, mServerIPAddress, mServerPort, userCryptoInfo);
            UserClient = result.Item1;
            return result.Item2;
        }

        public void CloseAndDisconnect() {
            try {
                if (UserClient != null) {
                    if (UserClient.Connected) {
                        UserClient.Close();
                        userInfo = new Packages.UserInfoPackage();
                        Debug.WriteLine("Client disconnected from server!");
                    }
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.ToString());
            }
        }

        public void SetTextBox(Label textBox) {
            appConsole = textBox;
        }

        public void SetPdfReader(PdfDocument pdf) {
            readerPdf = pdf;
        }

        public void SendText(string newText) {
            try {
                appConsole.Text = appConsole.Text + newText + Environment.NewLine;
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }

        }

        public void SendPdf(PdfLoadedDocument loadedDocument) {
            try {
                PdfDocumentBase.Merge(readerPdf, loadedDocument);
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }

        public bool CheckOngoingAuthorization(RSAUser crypto) {
            try {

                if (UserClient.Connected) {
                    timer.Start();
                    encryptTimer.Start();
                    RSAEncryptor encryptor = new RSAEncryptor(crypto.GetPrivateKey(), crypto.GetExternalPublicKey());
                    Packages.EncryptedPackage encrypted = encryptor.Encrypt(userInfo, typeof(Packages.UserInfoPackage));
                    encryptTimer.Stop();
                    serializer.XMLSerializer(UserClient.GetStream(), encrypted, encrypted.GetType());

                    decryptTimer.Start();
                    RSADecryptor decryptor = new RSADecryptor(crypto.GetPrivateKey(), crypto.GetExternalPublicKey());
                    encrypted = (Packages.EncryptedPackage)serializer.XMLDeserializer(UserClient.GetStream(), typeof(Packages.EncryptedPackage));
                    var RAMPackage = (Packages.ResourcePackage)decryptor.DecryptAndRead(encrypted, typeof(Packages.ResourcePackage));
                    decryptTimer.Stop();
                    //var RAMPackage = (Packages.ResourcePackage)serializer.BinaryDeserializer(UserClient.GetStream());
                    timer.Stop();
                    using (StreamWriter sw = File.AppendText(Path.Combine(androidFilepath, "ConnectionLog.txt")))
                    {
                        sw.WriteLine(count + "-----------------------------------------");
                        sw.WriteLine("Total time: " + String.Format("{0}", timer.Elapsed.Seconds * 1000 + timer.Elapsed.Milliseconds));
                        sw.WriteLine("Time to encrypt: " + encryptTimer.Elapsed.Milliseconds);
                        sw.WriteLine("Time to decrypt: " + decryptTimer.Elapsed.Milliseconds);
                        sw.Close();
                    }
                    timer.Reset();
                    encryptTimer.Reset();
                    decryptTimer.Reset();
                    count++;
                    Console.WriteLine("RAM Response: " + RAMPackage.Response);

                    if (RAMPackage.Response == "User not authorized to request resource.") {
                        SendText(RAMPackage.Response);
                        return false;
                    }
                    else if (RAMPackage.Response == "Outsize of STZone! Check your time and location of request.") {
                        SendText(RAMPackage.Response);
                        return false;
                    }
                    else { return true; }
                }
            }
            catch (Exception excp) {
                Debug.WriteLine(excp.ToString());
            }
            return false;
        }

        public async Task ReceiveFormRequest(int activity, int resource, int role, int locationChoice, int timeIndexChoice) {
            //consider refactoring switch statements to separate classes
            int activityID;
            switch (activity) {
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

        public async Task CredentialVerification(string username, string password, NetworkStream userStream, RSAUser crpytoInfo) {
            try
            {
                userInfo = credentialVerifier.VerifyCredentials(username, password, userStream, userInfo, crpytoInfo);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task requestResourcesFromServer(NetworkStream userStream, string systemFilepath, RSAUser rsaUser) {
            timer.Start();
            encryptTimer.Start();
            RSAEncryptor encryptor = new RSAEncryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
            Packages.EncryptedPackage encrypted = encryptor.Encrypt(userInfo, typeof(Packages.UserInfoPackage));
            encryptTimer.Stop();
            serializer.XMLSerializer(userStream, encrypted, encrypted.GetType());
            awaitPackage(userStream, systemFilepath, rsaUser);
        }

        public bool CheckIfValidRequest() {
            if (STZManager.getLocationID() != -1) { return true; }
            else { return false; }
        }

        public async Task awaitPackage(NetworkStream stream, string systemFilepath, RSAUser rsaUser) {
            Packages.ResourcePackage RAMPackage;

            try {
                decryptTimer.Start();
                RSADecryptor decryptor = new RSADecryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
                Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.XMLDeserializer(stream, typeof(Packages.EncryptedPackage));
                RAMPackage = (Packages.ResourcePackage)decryptor.DecryptAndRead(encrypted, typeof(Packages.ResourcePackage));
                decryptTimer.Stop();
                timer.Stop();
                //RAMPackage = (Packages.ResourcePackage)serializer.BinaryDeserializer(stream);

                using (StreamWriter sw = File.AppendText(Path.Combine(androidFilepath, "ConnectionLog.txt")))
                {
                    sw.WriteLine(count + "-----------------------------------------");
                    sw.WriteLine("Total time: " + String.Format("{0}", timer.Elapsed.Seconds * 1000 + timer.Elapsed.Milliseconds));
                    sw.WriteLine("Time to encrypt: " + encryptTimer.Elapsed.Milliseconds);
                    sw.WriteLine("Time to decrypt: " + decryptTimer.Elapsed.Milliseconds);
                    sw.Close();
                }
                timer.Reset();
                encryptTimer.Reset();
                decryptTimer.Reset();
                count++;
                if (RAMPackage.Response == "User not authorized to request resource.") {
                    SendText(RAMPackage.Response);
                    return;
                }
                else if (RAMPackage.Response == "Outsize of STZone! Check your time and location of request.") {
                    SendText(RAMPackage.Response);
                    return;
                }

                string filepath;
                if (userInfo.ResourceID == 3) {
                    filepath = Path.Combine(systemFilepath, "Response.pdf");
                }
                else {
                    filepath = Path.Combine(systemFilepath, "Response.txt");
                }

                if (userInfo.requestedActivity.ActivityID == (int)Packages.Actions.Read) {
                    if (userInfo.ResourceID == 3) {
                        //File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                        PdfLoadedDocument loadedDocument = new PdfLoadedDocument(RAMPackage.ResourceFile);
                        SendPdf(loadedDocument);
                    }
                    else {
                        File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                        string[] response = File.ReadAllLines(filepath);
                        for (int i = 0; i < response.Length; i++) {
                            SendText(response[i]);
                        }
                        File.Delete(filepath);
                    }
                }
                else if (userInfo.requestedActivity.ActivityID == (int)Packages.Actions.ReadWrite) {
                    File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                    string[] response = File.ReadAllLines(filepath);
                    for (int i = 0; i < response.Length; i++) {
                        SendText(response[i]);
                    }
                    await OnRaiseWriteToFileEvent(new WriteToFileEventArgs(filepath));
                }
                else if (userInfo.requestedActivity.ActivityID == (int)Packages.Actions.ReadWrite) {
                    File.WriteAllBytes(filepath, RAMPackage.ResourceFile);
                    await OnRaiseWriteToFileEvent(new WriteToFileEventArgs(filepath));
                }
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
