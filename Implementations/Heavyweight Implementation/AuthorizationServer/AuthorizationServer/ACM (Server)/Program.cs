using ACMSocketAsync;
using System;

namespace ACM__Server_ {
    class Program {
        static void Main(string[] args) {
            AuthorizationServer autherizationServer = new AuthorizationServer();
            Console.WriteLine("Welcome to GSTRBAC ACM Server!");
            autherizationServer.StartServer();

            while (true) {
                string userInput = null;
                Console.WriteLine("Type <EXIT> to stop the authorization server.");
                userInput = Console.ReadLine();
                if (userInput == "<EXIT>") {
                    Console.WriteLine("Shutting down Authorization Server...");
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadLine();
                    autherizationServer.StopServer();
                    break;
                }
            }
        }
    }
}
