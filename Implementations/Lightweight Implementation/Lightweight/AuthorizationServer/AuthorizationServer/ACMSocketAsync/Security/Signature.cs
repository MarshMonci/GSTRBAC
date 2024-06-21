using System;
using System.Security.Cryptography;
using Bouncy = Rebex.Security.Cryptography;

namespace ACMSocketAsync {
    public class Signature {
        byte[] key;
        byte[] signature;
        byte[] hash;
        Bouncy.EllipticCurveDsa signer;

        public Signature(byte[] senderKey) {
            signer = (Bouncy.EllipticCurveDsa)Bouncy.EllipticCurveDsa.Create(Bouncy.EllipticCurveDsa.EcDsaSha2Nistp256);
            key = senderKey;
        }

        public void getSignature(byte[] toSign, out byte[] hash, out byte[] signature) {
            SHA256 hasher = SHA256.Create();
            hash = hasher.ComputeHash(toSign);
            signature = signer.SignHash(hash);
        }

        public bool verifySignature(byte[] toVerify, byte[] hash, byte[] signature) {
            SHA256 hasher = SHA256.Create();

            if (hasher.ComputeHash(toVerify) != hash) {
                Console.WriteLine("Data has been changed!");
                return false;
            }
            else if (signer.VerifyHash(hash, signature) == false) {
                Console.WriteLine("Data has been changed!");
                return false;
            }
            Console.WriteLine("Data is valid");
            return true;
        }
    }
}
