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

namespace RAMSocketAsync
{
    public class AuthorizationRequester
    {
        string location = null;
        int time = 0;
        Packages serializer;
        IPAddress ACMIP;
        int ACMPort;
        TcpClient RAMclient;

        public TcpClient RAMClient
        {
            get { return RAMclient; }
            set
            {
                this.RAMclient = value;
            }
        }

        public AuthorizationRequester()
        {
            serializer = new Packages();
            this.RAMclient = new TcpClient();
        }

        public async Task sendSTZoneData (Packages.STZone zone)
        {
            if (zone == null)
            {
                Console.WriteLine("Empty string supplied to send.");
                return;
            }

            if (this.RAMclient != null)
            {
                if (this.RAMclient.Connected)
                {
                    Packages.ACMpackage STZoneData = new Packages.ACMpackage() { Stzone = zone };

                    serializer.BinarySerializer(this.RAMclient.GetStream(), STZoneData);
                }
            }
        }

        public int receiveToken()
        {
            try
            {
                int token = 0;

                Packages.Token ACMToken = (Packages.Token)serializer.BinaryDeserializer(this.RAMclient.GetStream());
                
                if (ACMToken.AuthToken == "1")
                    {
                        token = 1;
                        Console.WriteLine("Token granted.");
                    }
                else 
                    {
                        Console.WriteLine(ACMToken.AuthToken);
                    }
                this.RAMclient.Close();
                return token;
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
                return 0;
            }
        }

        public bool SetACMIPAddress(string IPAddressACM)
        {
            IPAddress ipaddr = null;

            //checks to see if IP address supplied is valid
            //if it isn't then the program will
            if (!IPAddress.TryParse(IPAddressACM, out ipaddr))
            {
                Console.WriteLine("Invalid server IP supplied.");
                return false;
            }


            ACMIP = ipaddr;
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
        //relocate to ATRequester class
        public async Task ConnectToACMServer()
        {
            if (this.RAMclient == null)
            {
                this.RAMclient = new TcpClient();
            }
            if (!this.RAMclient.Connected)
            {
                try
                {
                    this.RAMclient = new TcpClient();
                    await this.RAMclient.ConnectAsync(ACMIP, ACMPort);
                    if (this.RAMclient.Connected)
                    {
                        Console.WriteLine(string.Format("Connected to ACM server IP/Port: {0} / {1}",
                            ACMIP, ACMPort));
                    }
                }
                catch (Exception excp)
                {
                    Console.WriteLine(excp.ToString());
                    throw;
                }
            }
        }

        public async Task SendACMMessage(string receivedText)
        {
            //checks to see if string was actually supplied to method
            //returns immediately if a null or empty string was given
            if (string.IsNullOrEmpty(receivedText))
            {
                Console.WriteLine("Empty string supplied to send.");
                return;
            }

            if (this.RAMclient != null)
            {
                if (this.RAMclient.Connected)
                {
                    //StreamWriter is used to send data to the server
                    //I have no idea what AutoFlush does
                    //StreamWriter clientStreamWriter = new StreamWriter(RAMclient.GetStream());
                    //clientStreamWriter.AutoFlush = true;

                    //await clientStreamWriter.WriteAsync(receivedText);
                    byte[] buffMessage = Encoding.ASCII.GetBytes(receivedText);
                    this.RAMclient.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
                    Console.WriteLine("Data sent...");
                }
            }
        }
    }
}
