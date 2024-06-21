using System.Threading.Tasks;

namespace RAMSocketAsync.Resource_Components {
    public class TextFIleReadWriteActivity : Activity {
        Resource resource;

        public TextFIleReadWriteActivity(Resource newResource) {
            resource = newResource;
        }
        public async Task execute() {
            resource.readWrite();
        }
    }
}
