﻿using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SerializablePackages;
using System;

namespace MobileClient {
    public class AESEncryptor {

        private byte[] key;

        public AESEncryptor(byte[] key)
        {
            this.key = key;
        }

        public Packages.EncryptedPackage Encrypt(object toEncrpyt) {
            Packages.EncryptedPackage encrypted = new Packages.EncryptedPackage();

            using (Aes aes = Aes.Create()) {
                aes.Key = key;
                aes.Padding = PaddingMode.PKCS7;
                encrypted.iv = aes.IV;

                ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream()) {
                    using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write)) {
                        BinaryFormatter formatter = new BinaryFormatter();
                        MemoryStream stream = new MemoryStream();

                        formatter.Serialize(stream, toEncrpyt);

                        cs.Write(stream.GetBuffer(), 0, stream.GetBuffer().Length);
                        cs.Close();
                    }
                    encrypted.encryptedData = ms.ToArray();
                    ms.Close();

                    return encrypted;
                }
            }
        }
    }
}