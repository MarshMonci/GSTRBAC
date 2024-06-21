using SerializablePackages;

namespace ACMSocketAsync {
    public class PolicyChecker {
        private ZoneIdentifier zoneIdentifier;
        public STZoneChecker ZoneChecker { get; set; }

        public bool CheckPolicy(Packages.STZone stzone) {
            return (ZoneChecker.CheckSTZone(stzone));
        }

        public PolicyChecker() {
            zoneIdentifier = new ZoneIdentifier();
            ZoneChecker = new STZoneChecker(zoneIdentifier);
        }
    }
}
