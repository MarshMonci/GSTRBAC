using SerializablePackages;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace ACMSocketAsync {
    public class RSAEncryptor
    {
        private string privateKey;
        private string externalPublicKey;

        public RSAEncryptor(string privateKey, string externalPublicKey)
        {
            this.privateKey = privateKey;
            this.externalPublicKey = externalPublicKey;
        }

        public Packages.EncryptedPackage Encrypt(object obj, Type objT)
        {
            Packages.EncryptedPackage encrypted = new Packages.EncryptedPackage();

            byte[] objectData = ObjectToByteArray(obj, objT);
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(externalPublicKey);

                if (objectData.Length <= 244)
                {
                    encrypted.encryptedData = rsa.Encrypt(objectData, false);
                }
                else
                {

                    using (var plainStream = new MemoryStream(objectData))
                    {
                        using (var encryptStream = new MemoryStream())
                        {
                            var offSet = 0;
                            var inputLen = objectData.Length;
                            for (var i = 0; inputLen - offSet > 0; offSet = i * 244)
                            {
                                if (inputLen - offSet > 244)
                                {
                                    var buffer = new byte[244];
                                    plainStream.Read(buffer, 0, 244);
                                    var cryptograph = rsa.Encrypt(buffer, false);
                                    encryptStream.Write(cryptograph, 0, cryptograph.Length);
                                }
                                else
                                {
                                    var buffer = new byte[inputLen - offSet];
                                    plainStream.Read(buffer, 0, inputLen - offSet);
                                    var cryptograph = rsa.Encrypt(buffer, false);
                                    encryptStream.Write(cryptograph, 0, cryptograph.Length);
                                }
                                ++i;
                            }
                            encryptStream.Position = 0;
                            encrypted.encryptedData = encryptStream.ToArray();

                        }
                    }
                }
            }
            using (var rsaSign = new RSACryptoServiceProvider(2048))
            {
                rsaSign.FromXmlString(privateKey);
                encrypted.signature1 = rsaSign.SignData(encrypted.encryptedData, SHA256.Create());
            }
            return encrypted;
        }
        public byte[] ObjectToByteArray(object obj, Type objT)
        {
            XmlSerializer bf = new XmlSerializer(objT);
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}