using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.Net.Sockets;
using System.IO;

//UPDATED (NEW CLASS)
namespace RAMSocketAsync.Resource_Components
{
    public class TextFileUpdateFileActivity : Activity
    {
        Resource resource;
        Packages.UserInfoPackage userInfo;

        public TextFileUpdateFileActivity(Resource newResource, Packages.UserInfoPackage newUserInfo)
        {
            resource = newResource;
            userInfo = newUserInfo;
        }
        public async Task execute()
        {
            resource.updateFile(userInfo);
        }
    }
}
