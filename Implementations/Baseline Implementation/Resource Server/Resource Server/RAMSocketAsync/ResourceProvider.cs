using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SerializablePackages;
using System.Net.Sockets;

namespace RAMSocketAsync
{
    class ResourceProvider
    {
        ResourceDatabase database = null;
        public ResourceProvider(ResourceDatabase database)
        {
            this.database = database;
        }

        public string getResource(int request)
        {
            return database.getFile(request);
        }

        public async Task ReadActivity (string filepath, NetworkStream stream,
            Packages.ResourcePackage userPackage, Packages packer)
        {
            try
            {
                FileStream fstream = File.Open(filepath, FileMode.Open);
                fstream.Position = 0;

                byte[] fileData = new byte[fstream.Length];
                for (int i = 0; i < fileData.Length; i++)
                {
                    fileData[i] = (byte)fstream.ReadByte();
                }
                userPackage.ResourceFile = fileData;
                Console.WriteLine("File byte length: " + fileData.Length);
                packer.BinarySerializer(stream, userPackage);
                fstream.Close();
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
        }

        public async Task WriteActivity(string filepath, NetworkStream stream, Packages.ResourcePackage userPackage, Packages packer)
        {
            FileStream fstream = File.Open(filepath, FileMode.Open);
            fstream.Position = 0;

            byte[] fileData = new byte[fstream.Length];
            //assigning every single byte to file data to be sent to user
            for (int i = 0; i < fileData.Length; i++)
            {
                fileData[i] = (byte)fstream.ReadByte();
            }
            //assigns fileData to package to send to user
            userPackage.ResourceFile = fileData;
            Console.WriteLine("File byte length: " + fileData.Length);
            packer.BinarySerializer(stream, userPackage);
            fstream.Close();

            userPackage = (Packages.ResourcePackage)packer.BinaryDeserializer(stream);
            File.WriteAllBytes(filepath, userPackage.ResourceFile);
        }

        public async Task ReadWriteActivity(string filepath, NetworkStream stream,
            Packages.ResourcePackage userPackage, Packages packer)
        {
            FileStream fstream = File.Open(filepath, FileMode.Open);
            fstream.Position = 0;

            byte[] fileData = new byte[fstream.Length];
            for (int i = 0; i < fileData.Length; i++)
            {
                fileData[i] = (byte)fstream.ReadByte();
            }
            userPackage.ResourceFile = fileData;
            Console.WriteLine("File byte length: " + fileData.Length);
            packer.BinarySerializer(stream, userPackage);
            
            userPackage = (Packages.ResourcePackage)packer.BinaryDeserializer(stream);
            File.WriteAllBytes(filepath, userPackage.ResourceFile);
            fstream.Close();
        }
    }
}
