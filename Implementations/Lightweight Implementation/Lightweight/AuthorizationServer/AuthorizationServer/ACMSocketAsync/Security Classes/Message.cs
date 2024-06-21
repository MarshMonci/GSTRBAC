using System;
using System.Collections.Generic;
using System.Text;

namespace ACMSocketAsync {
    public class Message
    {
        [Serializable]
        public struct messagePackage
        {
            public byte[] encryptedMessage;
            public byte[] iv;
            public byte[] hash;
        }
    }
}
