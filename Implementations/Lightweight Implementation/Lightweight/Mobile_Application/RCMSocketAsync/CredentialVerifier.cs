using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;

namespace MobileClient
{
    public class CredentialVerifier
    {
        Packages serializer;
        Stopwatch requestTimer;
        public CredentialVerifier()
        {
            serializer = new Packages();
            requestTimer = new Stopwatch();
        }

        public Packages.UserInfoPackage VerifyCredentials(string username, string password, NetworkStream userStream, Packages.UserInfoPackage userInfo, ECDHUser cryptoInfo)
        {
            userInfo.Username = username;
            userInfo.Password = password;
            requestTimer.Restart();
            requestTimer.Start();

            AESEncryptor encryptor = new AESEncryptor(cryptoInfo.GetKey());
            Packages.EncryptedPackage encryptedPackage = encryptor.Encrypt(userInfo);

            Console.WriteLine("Sending Signature---------------------------------------------");
            try {
                //Swap for encryptedPackage instead of regular packaging
                serializer.BinarySerializer(userStream, encryptedPackage);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            AESDecryptor decryptor = new AESDecryptor(cryptoInfo.GetKey());
            encryptedPackage = (Packages.EncryptedPackage)serializer.BinaryDeserializer(userStream);
            requestTimer.Stop();
            userInfo = (Packages.UserInfoPackage)decryptor.DecryptAndRead(encryptedPackage);

            string elapsedTime = String.Format("{0}", requestTimer.Elapsed.Seconds * 1000 + requestTimer.Elapsed.Milliseconds);

            using(StreamWriter sWriter = File.AppendText(Path.Combine("/storage/emulated/0/Android/data/tamiu.gstrbac.mobile_app/files/", "ConnectionLog.txt"))) {
                sWriter.WriteLine("Log-in request duration: " + elapsedTime);
                Console.WriteLine("Log-in request duration: " + elapsedTime);
                sWriter.Close();
            }
            return userInfo;
        }
    }
}
