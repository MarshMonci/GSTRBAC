using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace RAMSocketAsync
{
    class RSADecryptor
    {
        private string privateKey;
        private string externalPublicKey;

        public RSADecryptor(string privateKey, string externalPublicKey)
        {
            this.privateKey = privateKey;
            this.externalPublicKey = externalPublicKey;
        }

        public object DecryptAndRead(SerializablePackages.Packages.EncryptedPackage encryptedPackage, Type objT)
        {
            if (encryptedPackage.encryptedData == null)
            {
                Console.WriteLine("Package has no data!");
            }
            if (encryptedPackage.signature1 == null)
            {
                Console.WriteLine("Package has no signature!");
            }
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(externalPublicKey);
                if (!rsa.VerifyData(encryptedPackage.encryptedData, SHA256.Create(), encryptedPackage.signature1))
                {
                    Console.WriteLine("Data not valid! Exiting Program...");
                    Environment.Exit(0);
                    return null;
                }
                else
                {

                    Console.WriteLine("Valid Signature");
                    Console.WriteLine("Decryption starts here");
                    byte[] decryptedBytes = null;
                    try
                    {
                        decryptedBytes = Decrypt(encryptedPackage.encryptedData);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    Console.WriteLine("Decryption ends here");
                    return ByteArrayToObject(decryptedBytes, objT);
                }
            }
        }

        public byte[] Decrypt(byte[] encryptedBytes)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(privateKey);
                if (encryptedBytes.Length <= rsa.KeySize / 8)
                {
                    return rsa.Decrypt(encryptedBytes, false);
                }
                else
                {
                    using (var plainStream = new MemoryStream(encryptedBytes))
                    {
                        using (var decryptStream = new MemoryStream())
                        {
                            var offSet = 0;
                            var inputLen = encryptedBytes.Length;
                            for (var i = 0; inputLen - offSet > 0; offSet = i * 256)
                            {
                                if (inputLen - offSet > 256)
                                {
                                    var buffer = new byte[256];
                                    plainStream.Read(buffer, 0, 256);
                                    var decrypData = rsa.Decrypt(buffer, false);
                                    decryptStream.Write(decrypData, 0, decrypData.Length);
                                }
                                else
                                {
                                    var buffer = new byte[inputLen - offSet];
                                    plainStream.Read(buffer, 0, inputLen - offSet);
                                    var decrypData = rsa.Decrypt(buffer, false);
                                    decryptStream.Write(decrypData, 0, decrypData.Length);
                                }
                                ++i;
                            }
                            decryptStream.Position = 0;
                            byte[] decryptedData = decryptStream.ToArray();
                            return decryptedData;
                        }
                    }
                }
            }
        }

        public object ByteArrayToObject(byte[] arrBytes, Type objT)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new XmlSerializer(objT);
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
