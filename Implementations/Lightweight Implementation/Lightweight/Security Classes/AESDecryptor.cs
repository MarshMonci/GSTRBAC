using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MobileClient {
    class AESDecryptor {
        private byte[] key;

        public AESDecryptor(byte[] key) {
            this.key = key;
        }

        public object DecryptAndRead(byte[] encryptedMessage, byte[] iv) {

            using (Aes aes = new AesCryptoServiceProvider()) {
                aes.Key = key;
                aes.IV = iv;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream()) {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();

                        Object obj = null;

                        //Deserializes package from Memory Stream
                        BinaryFormatter formatter = new BinaryFormatter();
                        plaintext.Position = 0;
                        obj = formatter.Deserialize(plaintext);
                        plaintext.Close();
                        return obj;
                    }
                }
            }
        }
    }
}
