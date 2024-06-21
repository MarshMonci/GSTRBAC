using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;

namespace MobileClient
{
    public class CredentialVerifier
    {
        Packages serializer;
        PasswordHasher hasher;
        Stopwatch requestTimer;
        public CredentialVerifier()
        {
            serializer = new Packages();
            hasher = new PasswordHasher();
            requestTimer = new Stopwatch();
        }

        public Packages.UserInfoPackage CredentialVerification(string username, string password, NetworkStream userStream, Packages.UserInfoPackage userInfo)
        {
            
            userInfo.Username = username;
            userInfo.Password = hasher.HashPassword(password, username);
            requestTimer.Restart();
            requestTimer.Start();
            try
            {
                serializer.XMLSerializer(userStream, userInfo, typeof(Packages.UserInfoPackage));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            userInfo = (Packages.UserInfoPackage)serializer.XMLDeserializer(userStream, typeof(Packages.UserInfoPackage));
            requestTimer.Stop();
            string elapsedTime = String.Format("{0}", requestTimer.Elapsed.Seconds * 1000 + requestTimer.Elapsed.Milliseconds);
            using (StreamWriter sWriter = File.AppendText(Path.Combine("/storage/emulated/0/Android/data/tamiu.gstrbac.mobile_app/files/", "ConnectionLog.txt")))
            {
                sWriter.WriteLine("Log-in request duration: " + elapsedTime);
                Console.WriteLine("Log-in request duration: " + elapsedTime);
                sWriter.Close();
            }
            return userInfo;
        }
    }
}
