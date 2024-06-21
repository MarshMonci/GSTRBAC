using SerializablePackages;
using System;
using System.Net.Sockets;
namespace RAMSocketAsync.AuthorizationComponents {
    public class TokenReceiver {
        TcpClient RAMclient;
        Packages serializer;

        public TokenReceiver(TcpClient RAMclient, Packages serializer) {
            this.RAMclient = RAMclient;
            this.serializer = serializer;
        }

        public int receiveToken(TcpClient client, RSAUser rsaUser) {
            try {
                RAMclient = client;
                int token = 0;
                RSADecryptor decryptor = new RSADecryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
                Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.XMLDeserializer(RAMclient.GetStream(), typeof(Packages.EncryptedPackage));
                Packages.Token ACMToken = (Packages.Token)decryptor.DecryptAndRead(encrypted, typeof(Packages.Token));
                if (ACMToken.AuthToken == "1") {
                    token = 1;
                    Console.WriteLine("Token granted.");
                }
                else {
                    Console.WriteLine(ACMToken.AuthToken);
                }
                RAMclient.Close();
                return token;
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
                return 1;
            }
        }
    }
}
