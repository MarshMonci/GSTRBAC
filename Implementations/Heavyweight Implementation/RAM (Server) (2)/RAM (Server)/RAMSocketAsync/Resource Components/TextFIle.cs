using SerializablePackages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RAMSocketAsync.Resource_Components {
    class TextFile : Resource {
        string filepath;
        NetworkStream stream;
        Packages.ResourcePackage userPackage;
        Packages packer;
        RSAUser ecdhUser;
        RSAEncryptor encryptor;

        public TextFile(string filepath, NetworkStream stream,
           Packages.ResourcePackage userPackage, Packages packer, RSAUser rsaUser) {
            this.filepath = filepath;
            this.stream = stream;
            this.userPackage = userPackage;
            this.packer = packer;
            this.ecdhUser = rsaUser;
            encryptor = new RSAEncryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());
        }

        public void read() {
            try {
                FileStream fstream = File.Open(filepath, FileMode.Open);
                fstream.Position = 0;

                byte[] fileData = new byte[fstream.Length];
                for (int i = 0; i < fileData.Length; i++) {
                    fileData[i] = (byte)fstream.ReadByte();
                }
                userPackage.ResourceFile = fileData;
                Console.WriteLine("File byte length: " + fileData.Length);

                Packages.EncryptedPackage encrypted = encryptor.Encrypt(userPackage, typeof(Packages.ResourcePackage));
                packer.XMLSerializer(stream, encrypted, encrypted.GetType());
                fstream.Close();
            }
            catch (Exception excp) {
                Console.WriteLine(excp.ToString());
            }
        }

        //public async Task write()
        //{
        //    FileStream fstream = File.Open(filepath, FileMode.Open);
        //    fstream.Position = 0;

        //    byte[] fileData = new byte[fstream.Length];
        //    //assigning every single byte to file data to be sent to user
        //    for (int i = 0; i < fileData.Length; i++)
        //    {
        //        fileData[i] = (byte)fstream.ReadByte();
        //    }
        //    //assigns fileData to package to send to user
        //    userPackage.ResourceFile = fileData;
        //    Console.WriteLine("File byte length: " + fileData.Length);
        //    packer.BinarySerializer(stream, userPackage);
        //    fstream.Close();

        //    //relocate to updateFile()
        //    userPackage = (Packages.ResourcePackage)packer.BinaryDeserializer(stream);
        //    File.WriteAllBytes(filepath, userPackage.ResourceFile);
        //}

        //public async Task readWrite()
        //{
        //    try
        //    {
        //        Console.Write("Hello");
        //        FileStream fstream = File.Open(filepath, FileMode.Open);
        //        fstream.Position = 0;

        //        byte[] fileData = new byte[fstream.Length];
        //        for (int i = 0; i < fileData.Length; i++)
        //        {
        //            fileData[i] = (byte)fstream.ReadByte();
        //        }
        //        userPackage.ResourceFile = fileData;
        //        Console.WriteLine("File byte length: " + fileData.Length);
        //        packer.BinarySerializer(stream, userPackage);

        //        //relocate to updateFile()
        //        userPackage = (Packages.ResourcePackage)packer.BinaryDeserializer(stream);
        //        File.WriteAllBytes(filepath, userPackage.ResourceFile);
        //        fstream.Close();
        //    }
        //    catch (Exception excp)
        //    {
        //        Console.WriteLine(excp.TargetSite);
        //    }

        //}

        public async Task write() {
            FileStream fstream = File.Open(filepath, FileMode.Open);
            fstream.Position = 0;

            byte[] fileData = new byte[fstream.Length];
            //assigning every single byte to file data to be sent to user
            for (int i = 0; i < fileData.Length; i++) {
                fileData[i] = (byte)fstream.ReadByte();
            }
            //assigns fileData to package to send to user
            userPackage.ResourceFile = fileData;
            Console.WriteLine("File byte length: " + fileData.Length);

            Packages.EncryptedPackage encrypted = encryptor.Encrypt(userPackage, typeof(Packages.ResourcePackage));
            packer.XMLSerializer(stream, encrypted, encrypted.GetType());
            fstream.Close();

            ////relocate to updateFile()
            //userPackage = (Packages.ResourcePackage)packer.BinaryDeserializer(stream);
            //File.WriteAllBytes(filepath, userPackage.ResourceFile);
        }

        public async Task readWrite() {
            try {
                Console.Write("Hello");
                FileStream fstream = File.Open(filepath, FileMode.Open);
                fstream.Position = 0;

                byte[] fileData = new byte[fstream.Length];
                for (int i = 0; i < fileData.Length; i++) {
                    fileData[i] = (byte)fstream.ReadByte();
                }
                userPackage.ResourceFile = fileData;
                Console.WriteLine("File byte length: " + fileData.Length);

                Packages.EncryptedPackage encrypted = encryptor.Encrypt(userPackage, typeof(Packages.ResourcePackage));
                packer.XMLSerializer(stream, encrypted, encrypted.GetType());
                fstream.Close();

                ////relocate to updateFile()
                //userPackage = (Packages.ResourcePackage)packer.BinaryDeserializer(stream);
                //File.WriteAllBytes(filepath, userPackage.ResourceFile);
            }
            catch (Exception excp) {
                Console.WriteLine(excp.TargetSite);
            }

        }

        //UPDATED
        public async Task updateFile(Packages.UserInfoPackage userFileUpdate) {
            try {
                File.WriteAllBytes(filepath, userFileUpdate.userFile);
            }
            catch (Exception excp) {
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
