using RAMSocketAsync.AuthorizationComponents;
using RAMSocketAsync.Resource_Components;
using SerializablePackages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RAMSocketAsync.ResourceServerComponents.ClientManagement {
    public class ClientManager {
        AuthorizationRequester authTokenRequester;
        ClientRemover clientRemover;
        Packages serializer;
        ResourceRequester resourceRequest;
        RSAUser rsaUser;
        public AuthorizationRequester AuthTokenRequester { get { return authTokenRequester; } }

        public ClientManager() {
            authTokenRequester = new AuthorizationRequester(this);
            serializer = new Packages();
        }

        public RSAUser GetRSA() {
            return rsaUser;
        }

        //Handshake should be started here, the handshake should be done immediately after connection is established
        public async Task ManageClient(TcpClient paramClient) {
            //used to read data from the network stream which is associated with
            //the TCP object passed to the method as a parameter
            NetworkStream stream = null;
            StreamReader reader = null;

            try {
                if (paramClient != null) {
                    stream = paramClient.GetStream();
                    reader = new StreamReader(stream);

                    rsaUser = new RSAUser();
                    ClientHandshake handshake = new ClientHandshake();
                    await handshake.Initialize(rsaUser, stream);
                    RSADecryptor decryptor = new RSADecryptor(rsaUser.GetPrivateKey(), rsaUser.GetExternalPublicKey());

                    Packages.UserInfoPackage userRequest;
                    //This should cast to encrypted package instead of userinfopackage first, then unpack into userinfopackage
                    Console.WriteLine("BEGIN TEST XML: ");

                    Packages.EncryptedPackage encryptedUserRequest = (Packages.EncryptedPackage)serializer.XMLDeserializer(stream, typeof(Packages.EncryptedPackage));
                    userRequest = new Packages.UserInfoPackage();
                    try
                    {
                        userRequest = (Packages.UserInfoPackage)decryptor.DecryptAndRead(encryptedUserRequest, typeof(Packages.UserInfoPackage));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    ACMUserChecker userRequestChecker = new ACMUserChecker(serializer, userRequest, paramClient);
                    userRequestChecker.checkUserStream(rsaUser);

                    resourceRequest = new ResourceRequester(authTokenRequester, userRequest, serializer, paramClient, rsaUser);
                    if (userRequestChecker.UserGrantedAccess) {
                        await resourceRequest.requestResource();
                    }
                }

            }
            catch (Exception excp) {
                clientRemover = new ClientRemover();
                clientRemover.RemoveClient(paramClient);
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
