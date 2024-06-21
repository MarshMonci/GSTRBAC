using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Sockets;
using SerializablePackages;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Parameters;

namespace MobileClient
{
    public class ServerHandshake
    {
        public async Task Initialize(ECDHUser ECDHUserPar, NetworkStream stream)
        {

            Packages.ECDHPackage package = new SerializablePackages.Packages.ECDHPackage();
            MessageUnpacker unpacker = new MessageUnpacker();
            Packages serializer = new Packages();

            byte[] keyToPass = ECDHUserPar.GetPublicKey();
            package.publicKey = keyToPass;
            serializer.BinarySerializer(stream, package);

            //byte[] buffer = new byte[397];
            package = (Packages.ECDHPackage)serializer.BinaryDeserializer(stream);
            ECDHUserPar.SetKey(package.publicKey);
        }
    }
}