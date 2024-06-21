using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Net;
using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;

namespace RAMSocketAsync.AuthorizationComponents
{
    public class AuthorizationRequester
    {
        Packages serializer;
        IPAddress ACMIP;
        int ACMPort;
        public TcpClient RAMclient; //UPDATED
        public ACMPackageSender acmPackageSender;//
        public TokenReceiver tokenReceiver;//
        public ECDHUser ecdhUser;
        IPAddress[] IPList;

        public TcpClient RAMClient
        {
            get { return RAMclient; }
            set
            {
                RAMclient = value;
            }
        }

        public AuthorizationRequester(RAMSocketAsync.ResourceServerComponents.ClientManagement.ClientManager manager)
        {
            serializer = new Packages();
            RAMclient = new TcpClient();
            acmPackageSender = new ACMPackageSender(serializer, RAMclient);
            tokenReceiver = new TokenReceiver(RAMClient, serializer);
            ecdhUser = manager.GetECDH();
            IPAddress[] newArray = new IPAddress[3];
            newArray[0] = IPAddress.Parse("23.98.141.50");
            newArray[1] = IPAddress.Parse("23.96.107.165");
            newArray[2] = IPAddress.Parse("52.250.0.114");

            IPList = newArray;
        }

        public bool SetACMIPAddress(string IPAddressACM) {
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
            Console.WriteLine("Checking available Authorization server hosts...");


            int RTT_MinIndex = 0;
            long RTT_Min = 5000;
            IPStatus Min_Status = new IPStatus();

            Ping prePing = new Ping();
            PingReply preReply;

            try {
                preReply = prePing.Send(IPList[0], 3000);

                if (preReply.Status == IPStatus.Success) {
                    Console.WriteLine("Ping to " + IPList[0].ToString() + "[" + preReply.Address.ToString() + "]" + "Response delay = " +
                        preReply.RoundtripTime.ToString() + " ms");
                    RTT_Min = preReply.RoundtripTime;
                }
                else {
                    Console.WriteLine("Error code: " + preReply.Status.ToString());
                    Console.WriteLine("Could not ping " + IPList[0].ToString() + "...");
                    RTT_Min = 6000;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
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
                        Console.WriteLine("Ping to " + IPList[i].ToString() + "[" + reply.Address.ToString() + "]" + "Response delay = " +
                            reply.RoundtripTime.ToString() + " ms");
                        if (reply.RoundtripTime < RTT) {
                            RTT = reply.RoundtripTime;
                            currentStatus = reply.Status;
                        }
                    }
                    else {
                        Console.WriteLine("Error code: " + reply.Status.ToString());
                        Console.WriteLine("Could not ping " + IPList[i].ToString() + "...");
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
                Console.WriteLine("Warning! Selected IP returns error code: " + Min_Status.ToString());
            }
            Console.WriteLine("Server being connected to is \"" + IPList[RTT_MinIndex].ToString() + "\"...");
            ACMIP = IPList[RTT_MinIndex];
            //Console.WriteLine("Server being connected to is \"" + "192.168.86.176" + "\"...");
            //ACMIP = IPAddress.Parse("23.98.141.50");
            return true;
        }

        public bool SetACMPortNumber(string ServerPort)
        {
            int portNumber = 0;

            //verifies if the port number is correct
            if (!int.TryParse(ServerPort.Trim(), out portNumber))
            {
                Console.WriteLine("Invalid port number supplied, return.");
                return false;
            }

            //verifies if the port number is in range
            if (portNumber <= 0 || portNumber > 65535)
            {
                Console.WriteLine("Port number must be between 0 and 65535.");
                return false;
            }

            ACMPort = portNumber;
            return true;
        }
        public async Task ConnectToACMServer()
        {
            if (!RAMclient.Connected)
            {
                try
                {
                    RAMclient = new TcpClient();
                    await this.RAMclient.ConnectAsync(ACMIP, ACMPort);
                    if (this.RAMclient.Connected)
                    {
                        Console.WriteLine(string.Format("Connected to ACM server IP/Port: {0} / {1}",
                            ACMIP, ACMPort));
                    }
                    else
                    {
                        Console.WriteLine("Failed to connect to server!");
                    }
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp.ToString());
                    Console.WriteLine(excp.StackTrace);
                }
            }
        }

        
    }
}
