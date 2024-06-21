using System;
using System.Security.Cryptography;
using Bouncy = Rebex.Security.Cryptography;
using SerializablePackages;
using System.Linq;

namespace MobileClient {
    public class Signature {
        byte[] key;
        byte[] signature;
        byte[] hash;
        Bouncy.EllipticCurveDsa signer;

        public Signature(byte[] senderKey) {
            signer = (Bouncy.EllipticCurveDsa)Bouncy.EllipticCurveDsa.Create(Bouncy.EllipticCurveDsa.EcDsaSha2Nistp256);
            key = senderKey;
        }

        public void getSignature(Packages.EncryptedPackage package, out byte[] signature, out byte[] key) {
            //SHA256 hasher = SHA256.Create();
            //hash = hasher.ComputeHash(package.encryptedData);
            signature = signer.SignMessage(package.encryptedData);
            //signature = signer.SignHash(hash);
            key = signer.GetPrivateKey();
        }

        public bool verifySignature(byte[] toVerify, byte[] hash) {
            SHA256 hasher = SHA256.Create();

            if (!hasher.ComputeHash(toVerify).SequenceEqual(hash)){
                Console.WriteLine("Data has been changed!");
                return false;
            }
            else {
                Console.WriteLine("Data is valid");
                return true;
            }
        }
    }
}
