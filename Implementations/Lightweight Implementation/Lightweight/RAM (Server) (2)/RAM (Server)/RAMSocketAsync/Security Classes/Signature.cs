using System;
using System.Security.Cryptography;
using Bouncy = Rebex.Security.Cryptography;
using SerializablePackages;
using System.Linq;

namespace RAMSocketAsync {
    public class Signature {
        byte[] key;
        byte[] signature;
        byte[] hash;
        Bouncy.EllipticCurveDsa signer;

        public Signature(byte[] senderKey) {
            signer = (Bouncy.EllipticCurveDsa)Bouncy.EllipticCurveDsa.Create(Bouncy.EllipticCurveDsa.EcDsaSha2Nistp256);
            key = senderKey;
        }

        public Packages.EncryptedPackage getSignature(Packages.EncryptedPackage package) {
            SHA256 hasher = SHA256.Create();
            hash = hasher.ComputeHash(package.encryptedData);
            //package.signature = signer.SignHash(hash);
            return package;
        }

        public bool verifySignature(byte[] toVerify, byte[] signature, byte[] key) {
            SHA256 hasher = SHA256.Create();

            signer.FromPrivateKey(key);

            if(signer.VerifyHash(hasher.ComputeHash(toVerify), signature)) {
                Console.WriteLine("Data is valid");
                return true;
            }
            else {
                Console.WriteLine("Data is invalid");
                return false;
            }
        }
    }
}
