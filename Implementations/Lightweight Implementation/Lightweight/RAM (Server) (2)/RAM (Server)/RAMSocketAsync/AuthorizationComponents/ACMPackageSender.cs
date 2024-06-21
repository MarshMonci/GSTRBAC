using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Net.Sockets;

namespace RAMSocketAsync.AuthorizationComponents
{
    public class ACMPackageSender
    {
        Packages serializer;
        TcpClient RAMclient;
        int activityID;
        string location;
        int roleID;
        int resourceID;
        int time;
        int userID;
        public ACMPackageSender(Packages serializer, TcpClient RAMclient)
        {
            this.serializer = serializer;
            this.RAMclient = RAMclient;
        }

        public int ActivityID { get; set; }
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public int ResourceID { get; set; }
        public string Location { get; set; }
        public int Time { get; set; }

        public async Task sendACMPackage(Packages.STZone zone, TcpClient client, ECDHUser ecdhUser)
        {
            RAMclient = client;
            if (zone == null)
            {
                Console.WriteLine("Empty string supplied to send.");
                return;
            }

            if (RAMclient != null && RAMclient.Connected)
            {
                Packages.ACMpackage STZoneData = new Packages.ACMpackage() { Stzone = zone };

                AESEncryptor encryptor = new AESEncryptor(ecdhUser.GetKey());
                Packages.EncryptedPackage encrypted = encryptor.Encrypt(STZoneData);
                serializer.BinarySerializer(this.RAMclient.GetStream(), encrypted);
            }
        }
    }
}
