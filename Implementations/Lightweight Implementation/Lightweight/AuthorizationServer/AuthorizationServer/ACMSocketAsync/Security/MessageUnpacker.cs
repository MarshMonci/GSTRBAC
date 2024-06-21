﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ACMSocketAsync {
    class MessageUnpacker {

        //Signature Verification should belong in this class
        //Change to take in an encrypted package rather than messagePackage
        public object unpackMessage(SerializablePackages.Packages.EncryptedPackage message, ECDHUser ECDHUserPar) {
            HashHandling hash = new HashHandling();
            AESDecryptor aes = new AESDecryptor(ECDHUserPar.GetKey());

            //Digital Signature verification should happen instead of verifying the hash
            if (true) {
                return null;
            }
            else {
                Console.WriteLine("Incoming message has been altered.");
                return null;
            }
        }
    }
}