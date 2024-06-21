using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ACMSocketAsync {
    class AESDecryptor {
        private byte[] key;

        public AESDecryptor(byte[] key) {
            this.key = key;
        }

        public object DecryptAndRead(SerializablePackages.Packages.EncryptedPackage encryptedPackage) {
            Signature signature = new Signature(key);

            if (signature.verifySignature(encryptedPackage.encryptedData, encryptedPackage.hash, encryptedPackage.signature)) {

                using (Aes aes = Aes.Create()) {
                    aes.Key = key;
                    aes.IV = encryptedPackage.iv;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream plaintext = new MemoryStream(encryptedPackage.encryptedData)) {
                        using (CryptoStream cs = new CryptoStream(plaintext, transform, CryptoStreamMode.Write)) {
                            cs.Write(encryptedPackage.encryptedData, 0, encryptedPackage.encryptedData.Length);
                            cs.FlushFinalBlock();

                            Object obj = null;
                            BinaryFormatter formatter = new BinaryFormatter();
                            plaintext.Position = 0;
                            obj = formatter.Deserialize(plaintext);
                            plaintext.Close();
                            return obj;
                        }
                    }
                }
            }
            else {
                return null;
            }
        }
    }
}
