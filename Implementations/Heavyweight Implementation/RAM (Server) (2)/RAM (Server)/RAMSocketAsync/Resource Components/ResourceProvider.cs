using RAMSocketAsync.ResourceServerComponents;

namespace RAMSocketAsync.Resource_Components {
    public class ResourceProvider {
        ResourceDatabase database;
        public ResourceProvider() {
        }

        //UPDATE
        public string getResource(int request) {
            return ResourceServer.database.getFile(request);
        }
    }
}
