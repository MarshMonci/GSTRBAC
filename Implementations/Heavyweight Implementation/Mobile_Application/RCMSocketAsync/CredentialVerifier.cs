using SerializablePackages;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace MobileClient {
    public class CredentialVerifier {
        Packages serializer;
        public CredentialVerifier() {
            serializer = new Packages();
        }

        public Packages.UserInfoPackage VerifyCredentials(string username, string password, NetworkStream userStream, Packages.UserInfoPackage userInfo, RSAUser cryptoInfo) {
            userInfo.Username = username;
            userInfo.Password = password;

            Packages.EncryptedPackage encryptedPackage = new Packages.EncryptedPackage();

            RSAEncryptor encryptor = new RSAEncryptor(cryptoInfo.GetPrivateKey(), cryptoInfo.GetExternalPublicKey());
            try {
                encryptedPackage = encryptor.Encrypt(userInfo, typeof(Packages.UserInfoPackage));
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }

            try {
                serializer.XMLSerializer(userStream, encryptedPackage, encryptedPackage.GetType());
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

            RSADecryptor decryptor = new RSADecryptor(cryptoInfo.GetPrivateKey(), cryptoInfo.GetExternalPublicKey());
            System.Threading.Thread.Sleep(1000);
            encryptedPackage = (Packages.EncryptedPackage)serializer.XMLDeserializer(userStream, typeof(Packages.EncryptedPackage));
            try {
                userInfo = (Packages.UserInfoPackage)decryptor.DecryptAndRead(encryptedPackage, typeof(Packages.UserInfoPackage));
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            return userInfo;
        }
    }
}
