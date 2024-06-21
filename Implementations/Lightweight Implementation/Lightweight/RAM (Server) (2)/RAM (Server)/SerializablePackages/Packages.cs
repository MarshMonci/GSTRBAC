using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Security.Cryptography;


namespace SerializablePackages {
    public class Packages
    {
        SHA256 hasher = SHA256.Create();
        static public List<STZone> ZoneList { get; set; } = new List<STZone>()
        {
            new STZone() { LocationID = 1, Location = "Home", StartTime = new DateTime(1, 1, 1, 17, 0, 0), StopTime = new DateTime(1, 1, 1, 7, 0, 0) },
            new STZone() { LocationID = 2, Location = "Development Office", StartTime = new DateTime(1, 1, 1, 7, 0, 0), StopTime = new DateTime(1, 1, 1, 17, 0, 0) },
            new STZone() { LocationID = 3, Location = "TestingOffice", StartTime = new DateTime(1, 1, 1, 7, 0, 0), StopTime = new DateTime(1, 1, 1, 17, 0, 0) },
            new STZone() { LocationID = 4, Location = "Director Office", StartTime = new DateTime(1, 1, 1, 7, 0, 0), StopTime = new DateTime(1, 1, 1, 17, 0, 0) },
            new STZone() { LocationID = 5, Location = "Department Building", StartTime = new DateTime(1, 1, 1, 7, 0, 0), StopTime = new DateTime(1, 1, 1, 17, 0, 0) },
            new STZone() { LocationID = 6, Location = "University of Texas", StartTime = new DateTime(1, 1, 1, 7, 0, 0), StopTime = new DateTime(1, 1, 1, 17, 0, 0) }
        };

        //maybe rename to userResourceRequest
        [Serializable]
        public struct UserInfoPackage
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public int RoleID { get; set; }
            //change resouce to int
            //each file has an integer identifier (file descriptor)
            public int ResourceID { get; set; }
            public string Resource { get; set; }
            //public STZone stzone;s
            public int LocationID { get; set; }
            //check what name is for timestamp in GSTRBAC paper
            public DateTime TimeStamp { get; set; }

            public Activity requestedActivity { get; set; }

            public bool correctCredentials { get; set; }
            public byte[] userFile { get; set; }
        }

        //will be updated to send user requested resources
        [Serializable]
        public struct ResourcePackage
        {
            public string Response { get; set; }
            public Activity requestedActivity { get; set; }
            public byte[] ResourceFile { get; set; }
        }

        //rename this struct later (maybe authTokenRequest)
        [Serializable]
        public struct ACMpackage
        {
            //Later include Activity and Resource (these are used for CheckAccess)
            //Add user ID
            public STZone Stzone { get; set; }
            public Activity requestedActivity { get; set; }
            //public string location;
            //public DateTime time;
            //public int time;
            //add user information
            //what object is the user trying to access?
        }

        [Serializable]
        public struct ECDHPackage {
            public byte[] publicKey;
        }

        [Serializable]
        public struct EncryptedPackage
        {
            public byte[] encryptedData; //This is the message being sent
            public byte[] iv; //The initialization vector for the sent message
        }

        [Serializable]
        public struct Token
        {
            public string AuthToken { get; set; } //change to bool
                                                  //specifies if access is granted or not

            //The following must be included in final token implementation
            //userID
            //tokenID
            //requestedRole
            //permissionsAssociatedToRole
            //STZone
        }

        [Serializable]
        public class STZone
        {

            static public int NumberOfObjects { get; set; }

            public string Location { get; set; }
            public int LocationID { get; set; }
            public DateTime StartTime { get; set; }//change to startTime
            public DateTime StopTime { get; set; }

            public STZone()
            {
                 NumberOfObjects++;
            }
        }

        public enum Actions
        {
            Read = 1,
            ReadWrite,
            Copy,
            Run,
            Review,
            UpdateFile
        }

        [Serializable]
        public class Activity
        {
            public int ActivityID {get; set;}

            public Activity(int action)
            {
                ActivityID = action;
            }
        }

        [Serializable]
        public class Resource
        {
            public string Filename { get; set; }
            public int ResourceID { get; set; }

            public Resource()
            {
            }
        }

        public Packages()
        {

        }

        public void BinarySerializer(NetworkStream stream, object package)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream mStream = new MemoryStream();
            //Serializes data into memory stream
            formatter.Serialize(mStream, package);

            //Calculates the byte length of the serialized package
            byte[] data = mStream.GetBuffer();
            int memSize = (int)mStream.Length;
            byte[] size = BitConverter.GetBytes(memSize);
            Console.WriteLine(memSize);

            //Writes length and then serialized package into the Network Stream
            stream.Write(size, 0, 4);
            Debug.WriteLine("Size data sent!");
            stream.Write(data, 0, memSize);
            Debug.WriteLine("Main data sent! Size of package is {0} bytes! {1}", memSize, data.Length);
            stream.Flush();
            mStream.Close();
        }

        public object BinaryDeserializer(NetworkStream stream)
        {
            MemoryStream mStream = new MemoryStream();
            //Receives length of expected package
            //This will allow the program to know when to stop reading
            byte[] data = new byte[4];
            int receive = stream.Read(data, 0, 4);
            int size = BitConverter.ToInt32(data, 0);
            int offset = 0;

            while (size > 0)
            {
                data = new byte[5000];
                //Takes data from Network Stream and places it into Memory Stream
                receive = stream.Read(data, 0, size);
                mStream.Write(data, offset, receive);
                offset += receive;
                size -= receive;
            }
            Object obj = null;
            //Deserializes package from Memory Stream
            BinaryFormatter formatter = new BinaryFormatter();
            mStream.Position = 0;
            try {
                obj = formatter.Deserialize(mStream);
                mStream.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            return obj;
        }

        public STZone AssignZone(int locationID, DateTime timeStamp)
        {
            foreach (STZone zone in ZoneList)
            {
                if (zone.LocationID == locationID)
                {
                    if (TimeSpan.Compare(zone.StartTime.TimeOfDay, zone.StopTime.TimeOfDay) > 0)
                    {
                        if (TimeSpan.Compare(timeStamp.TimeOfDay, zone.StartTime.TimeOfDay) < 0 &&
                        TimeSpan.Compare(zone.StopTime.TimeOfDay, timeStamp.TimeOfDay) < 0)
                        {
                            return zone;
                        }
                    }
                    else if (TimeSpan.Compare(timeStamp.TimeOfDay, zone.StartTime.TimeOfDay) >= 0 && 
                        TimeSpan.Compare(timeStamp.TimeOfDay, zone.StopTime.TimeOfDay) < 0)
                    {
                        return zone;
                    }
                }
            }
            //this acts as a dummy zone for purposes of comparison in other methods
            //consider creating a custom zone that has no permissions tied to it
            //with the same purpose in mind as a null location
            return new STZone() { LocationID = 0, Location = null};
        }
    }
}