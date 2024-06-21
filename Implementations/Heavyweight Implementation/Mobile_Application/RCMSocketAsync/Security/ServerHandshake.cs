using SerializablePackages;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MobileClient {
    public class ServerHandshake {
        public async Task<RSAUser> Initialize(RSAUser RSAUserPar, NetworkStream stream) {
            Packages.ECDHPackage package = new SerializablePackages.Packages.ECDHPackage();
            Packages serializer = new Packages();

            string keyToPass = RSAUserPar.GetPublicKey();
            package.publicKey = keyToPass;
            serializer.XMLSerializer(stream, package, package.GetType());

            package = (Packages.ECDHPackage)serializer.XMLDeserializer(stream, typeof(Packages.ECDHPackage));
            RSAUserPar.SetExternalPublicKey(package.publicKey);

            return RSAUserPar;
        }
    }
}