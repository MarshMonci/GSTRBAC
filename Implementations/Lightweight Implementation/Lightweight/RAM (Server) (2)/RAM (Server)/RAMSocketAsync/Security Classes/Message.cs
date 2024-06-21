using System;
using System.Collections.Generic;
using System.Text;

namespace RAMSocketAsync {
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
