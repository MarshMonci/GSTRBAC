using SerializablePackages;
using System.Threading.Tasks;

//UPDATED (NEW CLASS)
namespace RAMSocketAsync.Resource_Components {
    public class TextFileUpdateFileActivity : Activity {
        Resource resource;
        Packages.UserInfoPackage userInfo;

        public TextFileUpdateFileActivity(Resource newResource, Packages.UserInfoPackage newUserInfo) {
            resource = newResource;
            userInfo = newUserInfo;
        }
        public async Task execute() {
            resource.updateFile(userInfo);
        }
    }
}
