using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Net.Sockets;
namespace RAMSocketAsync.ResourceServerComponents.ClientManagement
{
    public class ACMUserChecker
    {
        bool userGrantedAccess;
        ClientRemover clientRemover;
        UserAuthenticator userAuthenticator;
        Packages serializer;
        Packages.UserInfoPackage userRequest;
        TcpClient paramClient;

        public bool UserGrantedAccess
        { get { return userGrantedAccess; } }

        public ACMUserChecker(Packages serializer, Packages.UserInfoPackage userRequest, TcpClient paramClient)
        {
            clientRemover = new ClientRemover();
            userAuthenticator = new UserAuthenticator();
            userGrantedAccess = false;
            this.serializer = serializer;
            this.userRequest = userRequest;
            this.paramClient = paramClient;
        }

        public void checkUserStream(ECDHUser eCDHUser)
        {
            System.Diagnostics.Debug.WriteLine("*** Ready to read");
            if (userAuthenticator.AuthenticateUser(userRequest.Username, userRequest.Password))
            {
                System.Diagnostics.Debug.WriteLine("*** Package's User: " + userRequest.Username);
                userRequest.correctCredentials = true;
                userGrantedAccess = true;
                AESEncryptor encryptor = new AESEncryptor(eCDHUser.GetKey());
                Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(userRequest);
                serializer.BinarySerializer(paramClient.GetStream(), encryptedPackage);
            }
            else
            {
                Packages.ResourcePackage userPackage = new Packages.ResourcePackage { Response = "Incorrect data submitted." };

                AESEncryptor encryptor = new AESEncryptor(eCDHUser.GetKey());
                Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(userPackage);
                serializer.BinarySerializer(paramClient.GetStream(), encryptedPackage);
                clientRemover.RemoveClient(paramClient);
            }
        }
    }
}
