using SerializablePackages;
using System.Net.Sockets;
namespace RAMSocketAsync.ResourceServerComponents.ClientManagement {
    public class ACMUserChecker {
        bool userGrantedAccess;
        ClientRemover clientRemover;
        UserAuthenticator userAuthenticator;
        Packages serializer;
        Packages.UserInfoPackage userRequest;
        TcpClient paramClient;

        public bool UserGrantedAccess { get { return userGrantedAccess; } }

        public ACMUserChecker(Packages serializer, Packages.UserInfoPackage userRequest, TcpClient paramClient) {
            clientRemover = new ClientRemover();
            userAuthenticator = new UserAuthenticator();
            userGrantedAccess = false;
            this.serializer = serializer;
            this.userRequest = userRequest;
            this.paramClient = paramClient;
        }

        public void checkUserStream(RSAUser RSAUser) {
            System.Diagnostics.Debug.WriteLine("*** Ready to read");
            if (userAuthenticator.AuthenticateUser(userRequest.Username, userRequest.Password)) {
                System.Diagnostics.Debug.WriteLine("*** Package's User: " + userRequest.Username);
                userRequest.correctCredentials = true;
                userGrantedAccess = true;
                RSAEncryptor encryptor = new RSAEncryptor(RSAUser.GetPrivateKey(), RSAUser.GetExternalPublicKey());
                Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(userRequest, typeof(Packages.UserInfoPackage));
                serializer.XMLSerializer(paramClient.GetStream(), encryptedPackage, encryptedPackage.GetType());
            }
            else {
                Packages.ResourcePackage userPackage = new Packages.ResourcePackage { Response = "Incorrect data submitted." };

                RSAEncryptor encryptor = new RSAEncryptor(RSAUser.GetPrivateKey(), RSAUser.GetExternalPublicKey());
                Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(userPackage, typeof(Packages.ResourcePackage));
                serializer.XMLSerializer(paramClient.GetStream(), encryptedPackage, encryptedPackage.GetType());
                clientRemover.RemoveClient(paramClient);
            }
        }
    }
}
