using SerializablePackages;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ACMSocketAsync {
    public class ClientHandshake {
        public async Task Initialize(RSAUser RSAUser, NetworkStream stream) {
            Packages.ECDHPackage package = new SerializablePackages.Packages.ECDHPackage();
            Packages serializer = new Packages();

            string keyToPass = RSAUser.GetPublicKey();
            package.publicKey = keyToPass;
            serializer.XMLSerializer(stream, package, package.GetType());

            //byte[] buffer = new byte[397];
            package = (Packages.ECDHPackage)serializer.XMLDeserializer(stream, typeof(Packages.ECDHPackage));
            RSAUser.SetExternalPublicKey(package.publicKey);
        }
    }
}