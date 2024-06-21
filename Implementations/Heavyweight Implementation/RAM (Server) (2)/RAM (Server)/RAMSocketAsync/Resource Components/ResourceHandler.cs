using SerializablePackages;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RAMSocketAsync.Resource_Components {
    class ResourceHandler {
        NetworkStream stream;
        ResourceProvider resourceProvider;
        Packages serializer;
        Packages.UserInfoPackage userRequest;
        RSAUser ecdhUser;
        public ResourceHandler(NetworkStream stream, Packages serializer, Packages.UserInfoPackage userRequest, RSAUser ecdhUser) {
            this.stream = stream;
            resourceProvider = new ResourceProvider();
            this.serializer = serializer;
            this.userRequest = userRequest;
            this.ecdhUser = ecdhUser;
        }

        public Task handleResource() {

            Packages.ResourcePackage userPackage = new Packages.ResourcePackage() { Response = "Token has been granted by the ACM." };

            //UPDATE
            string filepath = resourceProvider.getResource(userRequest.ResourceID);
            Activity activity;
            Resource resource;

            resource = new TextFile(filepath, stream, userPackage, serializer, ecdhUser);

            if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.Read)) {
                activity = new TextFIleReadActivity(resource);
                activity.execute();
            }
            else if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.ReadWrite)) //UPDATED
            {
                activity = new TextFIleWriteActivity(resource);
                activity.execute();
            }
            else if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.ReadWrite)) {
                activity = new TextFIleReadWriteActivity(resource);
                activity.execute();
            }
            else if (userRequest.requestedActivity.ActivityID == Convert.ToInt32(Packages.Actions.UpdateFile)) //UPDATED
            {
                activity = new TextFileUpdateFileActivity(resource, userRequest);
                activity.execute();
            }
            return Task.FromResult(0);
        }
    }
}
