using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace MobileClient {
    public class AESEncryptor {

        private byte[] key;

        public AESEncryptor(byte[] key)
        {
            this.key = key;
        }

        public void Encrypt(string messageToSend, out byte[] encryptedMessage, out byte[] iv) {
            using (Aes aes = new AesCryptoServiceProvider()) {
                aes.Key = key;
                iv = aes.IV;
                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(messageToSend);
                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    encryptedMessage = ciphertext.ToArray();
                }
            }
        }
    }
}
