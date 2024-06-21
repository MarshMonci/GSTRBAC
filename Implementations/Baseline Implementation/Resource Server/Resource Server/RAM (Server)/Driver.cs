using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using RAMSocketAsync;
using System.Runtime.Serialization.Formatters;

namespace RAM__Server_
{
    class Driver
    {
        static void Main(string[] args)
        {
            ResourceServer RAMServer = new ResourceServer();

            Console.WriteLine("Welcome to GSTRBAC RAM Server!");
            RAMServer.authTokenRequester.SetACMIPAddress("127.0.0.1");
            RAMServer.authTokenRequester.SetACMPortNumber("24000");
            //Console.WriteLine("Please input IP address of ACM server: ");
            ////reads user input for IP address
            //string strIPAddress = Console.ReadLine();

            //Console.WriteLine("Please Supply a Valid Port Number 0 - 65535 and Press Enter: ");
            ////reads user input for port number
            //string strPortInput = Console.ReadLine();

            ////verifies if the server information is correct
            ////if not, the program ends
            //if (!RAMServer.SetACMIPAddress(strIPAddress) ||
            //    !RAMServer.SetACMPortNumber(strPortInput))
            //{
            //    Console.WriteLine(String.Format("Wrong IP Address or port number supplied - {0} - {1} - Press a key to exit",
            //        strIPAddress, strPortInput));
            //    Console.ReadKey();
            //    return;
            //}
            RAMServer.ListenForConnections();
            
            while(true)
            {
                string userInput = null;
                Console.WriteLine("Type <EXIT> to close the program.");
                userInput = Console.ReadLine();
                if (userInput == "<EXIT>")
                {
                    break;
                }
            }
        }
    }
}
