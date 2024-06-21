using RAMSocketAsync.ResourceServerComponents;
using System;

namespace RAM__Server_ 
{
    class Driver {
        static void Main(string[] args) {
            ResourceServer RAMServer = new ResourceServer();
            Console.WriteLine("Welcome to GSTRBAC RAM Server!");
            RAMServer.StartServer("127.0.0.1", "24000");

            while (true) {
                string userInput = null;
                Console.WriteLine("Type <EXIT> to close the program.");
                userInput = Console.ReadLine();
                if (userInput == "<EXIT>") {
                    RAMServer.StopServer();
                    break;
                }
            }
        }
    }
}