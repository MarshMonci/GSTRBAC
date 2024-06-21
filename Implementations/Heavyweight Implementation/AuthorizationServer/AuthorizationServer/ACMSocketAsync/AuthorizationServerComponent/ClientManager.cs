using SerializablePackages;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ACMSocketAsync {
    public class ClientManager {
        Packages serializer;
        AuthorizationProvider ATGranter;
        Packages.Resource accessedResource;
        Activity userAction;
        //TcpClient RAMClient;
        ClientRemover clientRemover;
        RSAUser rsaUser;

        public ClientManager(AuthorizationProvider ATGranter) {
            serializer = new Packages();
            this.ATGranter = ATGranter;
            accessedResource = new Packages.Resource();
            userAction = new Activity();
            //this.RAMClient = RAMClient;
            clientRemover = new ClientRemover();
            rsaUser = new RSAUser();
        }

        public async Task ManageClient(TcpClient resourceClient) {

            NetworkStream stream = null;
            StreamReader reader = null;

            try {
                stream = resourceClient.GetStream();
                reader = new StreamReader(stream);
                ClientHandshake handshake = new ClientHandshake();
                await handshake.Initialize(rsaUser, stream);

                while (AuthorizationServer.keepRunning) {
                    RSADecryptor decryptor = new RSADecryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
                    Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.XMLDeserializer(stream, typeof(Packages.EncryptedPackage));
                    Packages.ACMpackage userSTZoneData = (Packages.ACMpackage)decryptor.DecryptAndRead(encrypted, typeof(Packages.ACMpackage));

                    //Packages.ACMpackage userSTZoneData = (Packages.ACMpackage)serializer.BinaryDeserializer(resourceClient.GetStream());
                    if (userSTZoneData.Stzone != null && ATGranter.checkAccess(accessedResource, userAction, userSTZoneData.Stzone)) {
                        Packages.Token token = new Packages.Token() { AuthToken = "1" };
                        RSAEncryptor encryptor = new RSAEncryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
                        Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(token, typeof(Packages.Token));
                        serializer.XMLSerializer(resourceClient.GetStream(), encryptedPackage, encryptedPackage.GetType());
                        break;
                    }
                    else if (userSTZoneData.Stzone == null || !ATGranter.checkAccess(accessedResource, userAction, userSTZoneData.Stzone)) {
                        Packages.Token token = new Packages.Token() { AuthToken = "Unauthorized STZone." };
                        RSAEncryptor encryptor = new RSAEncryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
                        Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(token, typeof(Packages.Token));
                        serializer.XMLSerializer(resourceClient.GetStream(), encryptedPackage, encryptedPackage.GetType());
                        break;
                    }

                }
            }
            catch (Exception excp) {
                clientRemover.RemoveClient(resourceClient);
                Console.WriteLine(excp.ToString());
            }

            if (resourceClient != null) {
                try {
                    clientRemover.RemoveClient(resourceClient);
                    Console.WriteLine("RAM Server disconnected.");
                }
                catch (Exception excp) {
                    Debug.WriteLine(excp.ToString());
                    Console.WriteLine("RAM Server disconnected on its own.");
                }
            }
        }
    }
}
