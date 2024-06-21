using System;
using System.Security.Cryptography;
using bouncy = Rebex.Security.Cryptography;

namespace ACMSocketAsync {
    //Find new class name (ECDHExchanger)
    public class RSAUser
    {
        private RSACryptoServiceProvider user;
        private string publicKey;
        private string privateKey;
        private string extPublicKey;
        private int keySize = 2048;

        public RSAUser()
        {
            user = new RSACryptoServiceProvider(keySize);
            publicKey = user.ToXmlString(false);
            privateKey = user.ToXmlString(true);
        }




        public string GetPublicKey() {
            return publicKey;
        }

        public string GetPrivateKey()
        {
            return privateKey;
        }
        public byte[] sign(byte[] toSign) {
            return user.SignData(toSign, SHA256.Create());
        }

        public bool verifySign(byte[] toCheck, byte[] signature) {
            return user.VerifyData(toCheck, SHA256.Create(), signature);
        }

        public void SetExternalPublicKey(string publicKey) {
            this.extPublicKey = publicKey;
            Console.WriteLine("Shared key set: " + this.extPublicKey.Length);
        }
        
        public string GetExternalPublicKey()
        {
            return extPublicKey;
        }
    }
}
