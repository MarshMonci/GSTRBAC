using System.Threading.Tasks;

namespace RAMSocketAsync.Resource_Components {
    public class TextFIleWriteActivity : Activity {
        Resource resource;

        public TextFIleWriteActivity(Resource newResource) {
            resource = newResource;
        }
        public async Task execute() {
            await resource.write();
        }
    }
}
