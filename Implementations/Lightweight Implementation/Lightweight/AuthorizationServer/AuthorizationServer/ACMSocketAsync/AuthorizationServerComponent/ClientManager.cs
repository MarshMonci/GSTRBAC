using SerializablePackages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ACMSocketAsync
{
    public class ClientManager
    {
        Packages serializer;
        AuthorizationProvider ATGranter;
        Packages.Resource accessedResource;
        Activity userAction;
        //TcpClient RAMClient;
        ClientRemover clientRemover;
        ECDHUser ecdhUser;

        public ClientManager(AuthorizationProvider ATGranter)
        {
            serializer = new Packages();
            this.ATGranter = ATGranter; 
            accessedResource = new Packages.Resource();
            userAction = new Activity();
            //this.RAMClient = RAMClient;
            clientRemover = new ClientRemover();
            ecdhUser = new ECDHUser();
        }

        public async Task ManageClient(TcpClient resourceClient)
        {

            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = resourceClient.GetStream();
                reader = new StreamReader(stream);
                ClientHandshake handshake = new ClientHandshake();
                await handshake.Initialize(ecdhUser, stream);

                while (AuthorizationServer.keepRunning)
                {
                    AESDecryptor decryptor = new AESDecryptor(ecdhUser.GetKey());
                    Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.BinaryDeserializer(stream);
                    Packages.ACMpackage userSTZoneData = (Packages.ACMpackage)decryptor.DecryptAndRead(encrypted);

                    //Packages.ACMpackage userSTZoneData = (Packages.ACMpackage)serializer.BinaryDeserializer(resourceClient.GetStream());
                    if (userSTZoneData.Stzone != null && ATGranter.checkAccess(accessedResource, userAction, userSTZoneData.Stzone))
                    {
                        Packages.Token token = new Packages.Token() { AuthToken = "1" };
                        AESEncryptor encryptor = new AESEncryptor(ecdhUser.GetKey());
                        Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(token);
                        serializer.BinarySerializer(resourceClient.GetStream(), encryptedPackage);
                        break;
                    }
                    else if (userSTZoneData.Stzone == null || !ATGranter.checkAccess(accessedResource, userAction, userSTZoneData.Stzone))
                    {
                        Packages.Token token = new Packages.Token() { AuthToken = "Unauthorized STZone." };
                        AESEncryptor encryptor = new AESEncryptor(ecdhUser.GetKey());
                        Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(token);
                        serializer.BinarySerializer(resourceClient.GetStream(), encryptedPackage);
                        break;
                    }

                }
            }
            catch (Exception excp)
            {
                clientRemover.RemoveClient(resourceClient);
                Console.WriteLine(excp.ToString());
            }

            if (resourceClient != null)
            {
                try
                {
                    clientRemover.RemoveClient(resourceClient);
                    Console.WriteLine("RAM Server disconnected.");
                }
                catch (Exception excp)
                {
                    Debug.WriteLine(excp.ToString());
                    Console.WriteLine("RAM Server disconnected on its own.");
                }
            }
        }
    }
}
