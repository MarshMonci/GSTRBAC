using SerializablePackages;

namespace ACMSocketAsync {
    public class AuthorizationProvider {
        public bool checkAccess(Packages.Resource r, Activity a, Packages.STZone stzone) {
            return (AuthorizationServer.Checker.CheckPolicy(stzone));
        }
    }

    public class Activity {
        public bool exists;

        public Activity() {
            exists = true;
        }
    }
}
