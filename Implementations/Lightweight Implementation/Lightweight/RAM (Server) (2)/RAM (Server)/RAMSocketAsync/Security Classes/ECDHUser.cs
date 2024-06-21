using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using bouncy = Rebex.Security.Cryptography;
using System.Collections.Generic;

namespace RAMSocketAsync {
    //Find new class name (ECDHExchanger)
    public class ECDHUser //: ECDiffieHellmanCng (Possibly refactor for inheritance of the ECDHCng Class later)
    {
        private bouncy.EllipticCurveDiffieHellman user;
        private bouncy.EllipticCurveDsa signature;
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

        public byte[] sign(byte[] toSign) {
            return signature.SignMessage(toSign);
        }

        public bool verifySign(byte[] toCheck, byte[] signature) {
            return this.signature.VerifyMessage(toCheck, signature);
        }

        public void SetKey(byte[] keyBlob)
        {
            this.key = user.GetSharedSecret(keyBlob);
            Console.WriteLine("Shared key set: " + this.key.Length);
        }
    }
}
