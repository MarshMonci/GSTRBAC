using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using bouncy = Rebex.Security.Cryptography;

namespace MobileClient
{
    //Find new class name (ECDHExchanger)
    public class ECDHUser //: ECDiffieHellmanCng (Possibly refactor for inheritance of the ECDHCng Class later)
    {
        private bouncy.EllipticCurveDiffieHellman user;
        private Aes aesAlgorithm;
        private byte[] key;
        public byte[] publicKey;
        public ECDHUser()
        {
            user = (bouncy.EllipticCurveDiffieHellman)bouncy.EllipticCurveAlgorithm.Create(bouncy.EllipticCurveDiffieHellman.EcdhSha2BrainpoolP512R1);
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
