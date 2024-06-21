using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;
using SerializablePackages;
using System.IO;
using RAMSocketAsync.AuthorizationComponents;
using RAMSocketAsync.Resource_Components;

namespace RAMSocketAsync.ResourceServerComponents.ClientManagement
{
    public class ClientManager
    {
        AuthorizationRequester authTokenRequester;
        ClientRemover clientRemover;
        Packages serializer;
        ResourceRequester resourceRequest;
        ECDHUser ecdhUser;
        public AuthorizationRequester AuthTokenRequester
        { get { return authTokenRequester; } }

        public ClientManager()
        {
            authTokenRequester = new AuthorizationRequester(this);
            serializer = new Packages();
        }

        public ECDHUser GetECDH() {
            return ecdhUser;
        }

        //Handshake should be started here, the handshake should be done immediately after connection is established
        public async Task ManageClient(TcpClient paramClient)
        {
            //used to read data from the network stream which is associated with
            //the TCP object passed to the method as a parameter
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                if(paramClient != null)
                {
                    stream = paramClient.GetStream();
                    reader = new StreamReader(stream);

                    ecdhUser = new ECDHUser();
                    ClientHandshake handshake = new ClientHandshake();
                    await handshake.Initialize(ecdhUser, stream);
                    AESDecryptor decryptor = new AESDecryptor(ecdhUser.GetKey());

                    Packages.UserInfoPackage userRequest;
                    //This should cast to encrypted package instead of userinfopackage first, then unpack into userinfopackage
                    Packages.EncryptedPackage encrypedUserRequest = (Packages.EncryptedPackage)serializer.BinaryDeserializer(paramClient.GetStream());
                    userRequest = (Packages.UserInfoPackage)decryptor.DecryptAndRead(encrypedUserRequest);

                    Console.WriteLine("Done with decryption");

                    ACMUserChecker userRequestChecker = new ACMUserChecker(serializer, userRequest, paramClient);
                    userRequestChecker.checkUserStream(ecdhUser);

                    resourceRequest = new ResourceRequester(authTokenRequester, userRequest, serializer, paramClient, ecdhUser);
                    if (userRequestChecker.UserGrantedAccess)
                    {
                        await resourceRequest.requestResource();
                    }
                }
                
            }
            catch (Exception excp)
            {
                clientRemover = new ClientRemover();
                clientRemover.RemoveClient(paramClient);
                System.Diagnostics.Debug.WriteLine(excp.ToString());
            }
        }
    }
}
