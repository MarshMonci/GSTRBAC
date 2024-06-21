using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Net.Sockets;
namespace RAMSocketAsync.AuthorizationComponents
{
    public class TokenReceiver 
    {
        TcpClient RAMclient;
        Packages serializer;

        public TokenReceiver(TcpClient RAMclient, Packages serializer)
        {
            this.RAMclient = RAMclient;
            this.serializer = serializer;
        }

        public int receiveToken(TcpClient client, ECDHUser ecdhUser)
        {
            try
            {
                RAMclient = client;
                int token = 0;
                AESDecryptor decryptor = new AESDecryptor(ecdhUser.GetKey());
                Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.BinaryDeserializer(RAMclient.GetStream());
                Packages.Token ACMToken = (Packages.Token)decryptor.DecryptAndRead(encrypted);
                if (ACMToken.AuthToken == "1")
                {
                    token = 1;
                    Console.WriteLine("Token granted.");
                }
                else
                {
                    Console.WriteLine(ACMToken.AuthToken);
                }
                RAMclient.Close();
                return token;
            }
            catch (Exception excp)
            {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
                return 1;
            }
        }
    }
}
