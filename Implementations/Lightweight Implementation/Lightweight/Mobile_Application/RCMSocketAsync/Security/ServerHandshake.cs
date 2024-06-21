using System.Threading.Tasks;
using System.Net.Sockets;
using SerializablePackages;

namespace MobileClient {
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