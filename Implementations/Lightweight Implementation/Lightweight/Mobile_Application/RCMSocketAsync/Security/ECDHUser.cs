using System;
using System.Security.Cryptography;
using bouncy = Rebex.Security.Cryptography;
using System.Collections.Generic;

namespace MobileClient {
    //Find new class name (ECDHExchanger)
    public class ECDHUser
    {
        private bouncy.EllipticCurveDiffieHellman user;
        private Aes aesAlgorithm;
        private byte[] key;
        public byte[] publicKey;

        public ECDHUser()
        {
            user = (bouncy.EllipticCurveDiffieHellman)bouncy.EllipticCurveAlgorithm.Create(bouncy.EllipticCurveDiffieHellman.EcdhSha2BrainpoolP256R1);
            aesAlgorithm = new AesCryptoServiceProvider();
            publicKey = user.GetPublicKey();
        }

        public byte[] GetPublicKey()
        {
            return publicKey;
        }

        public byte[] GetKey()
        {
            return this.key;
        }

        public void SetKey(byte[] keyBlob)
        {
            this.key = user.GetSharedSecret(keyBlob);
        }
    }
}
