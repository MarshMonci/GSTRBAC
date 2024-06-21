using System.Threading.Tasks;

namespace RAMSocketAsync.Resource_Components {
    public class TextFIleReadActivity : Activity {
        Resource resource;

        public TextFIleReadActivity(Resource newResource) {
            resource = newResource;
        }
        public Task execute() {
            resource.read();
            return Task.FromResult(0);
        }
    }
}
