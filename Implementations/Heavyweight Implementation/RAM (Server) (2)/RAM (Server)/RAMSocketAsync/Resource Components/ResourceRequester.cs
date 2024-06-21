using RAMSocketAsync.AuthorizationComponents;
using RAMSocketAsync.ResourceServerComponents;
using SerializablePackages;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace RAMSocketAsync.Resource_Components {
    class ResourceRequester {
        AuthorizationRequester authTokenRequester;
        Packages serializer;
        Packages.UserInfoPackage userRequest;
        TcpClient paramClient;
        RSAUser resourceECDH;
        RSAUser clientECDH;
        RSAEncryptor clientEncryptor;
        RSADecryptor clientDecryptor;
        RSADecryptor resourceDecryptor;
        RSAEncryptor resourceEncryptor;


        public ResourceRequester(AuthorizationRequester authTokenRequester, Packages.UserInfoPackage userRequest, Packages serializer, TcpClient paramClient, RSAUser clientRSA) {
            this.authTokenRequester = authTokenRequester;
            this.userRequest = userRequest;
            this.serializer = serializer;
            this.paramClient = paramClient;
            this.resourceECDH = new RSAUser();
            this.clientECDH = clientRSA;
            this.clientDecryptor = new RSADecryptor(clientRSA.GetPrivateKey(), clientRSA.GetExternalPublicKey());
        }
        public async Task requestResource() {
            int accessGranted = 0;
            ResourceHandler resourceHandler;
            Stopwatch requestTimer = new Stopwatch();
            Console.WriteLine("Requesting Resources");
            while (ResourceServer.keepRunning) {
                //Main bulk of the user request data is now received
                try {
                    Packages.EncryptedPackage encrypted = (Packages.EncryptedPackage)serializer.XMLDeserializerWithNoNulls(paramClient.GetStream(), typeof(Packages.EncryptedPackage));
                    userRequest = (Packages.UserInfoPackage)clientDecryptor.DecryptAndRead(encrypted, typeof(Packages.UserInfoPackage));
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
                //userRequest = (Packages.UserInfoPackage)serializer.BinaryDeserializer(paramClient.GetStream());

                if (!authTokenRequester.RAMclient.Connected) {
                    await authTokenRequester.ConnectToACMServer();
                }

                ClientHandshake handshake = new ClientHandshake();
                await handshake.Initialize(resourceECDH, authTokenRequester.RAMclient.GetStream());

                //UPDATE
                Packages.STZone userZone = serializer.AssignZone(userRequest.LocationID, userRequest.TimeStamp); //UPDATED

                Console.WriteLine("Requesting token for user...");

                requestTimer.Restart();
                requestTimer.Start();

                await authTokenRequester.acmPackageSender.sendACMPackage(userZone, authTokenRequester.RAMclient, resourceECDH);
                accessGranted = authTokenRequester.tokenReceiver.receiveToken(authTokenRequester.RAMclient, resourceECDH); //UPDATED

                requestTimer.Stop();
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", requestTimer.Elapsed.Minutes, requestTimer.Elapsed.Seconds,
                    requestTimer.Elapsed.Milliseconds);
                Console.WriteLine("Authorization request duration: " + elapsedTime);

                //verifies the user's zone
                if (userZone.Location == "Null") {
                    Packages.ResourcePackage userPackage = new Packages.ResourcePackage() {
                        Response = "Outsize of STZone! Check your time and location" +
                        " of request."
                    };
                    Packages.EncryptedPackage encryptedPackage = clientEncryptor.Encrypt(userPackage, typeof(Packages.ResourcePackage));
                    serializer.XMLSerializer(paramClient.GetStream(), encryptedPackage, encryptedPackage.GetType());
                }
                else if (accessGranted == 1) {
                    resourceHandler = new ResourceHandler(paramClient.GetStream(), serializer, userRequest, clientECDH);
                    await resourceHandler.handleResource();
                }
                else if (accessGranted == 0) {
                    Packages.ResourcePackage userPackage = new Packages.ResourcePackage() { Response = "User not authorized to request resource." };
                    Packages.EncryptedPackage encryptedPackage = clientEncryptor.Encrypt(userPackage, typeof(Packages.ResourcePackage));
                    serializer.XMLSerializer(paramClient.GetStream(), encryptedPackage, encryptedPackage.GetType());
                }
            }
        }
    }
}
