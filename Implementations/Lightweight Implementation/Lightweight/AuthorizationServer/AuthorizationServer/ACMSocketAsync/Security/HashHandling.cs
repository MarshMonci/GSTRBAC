﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ACMSocketAsync {
    public class HashHandling {

        public byte[] computeHash(byte[] toHash) {
            SHA512Managed sha = new SHA512Managed();

            toHash = sha.ComputeHash(toHash);

            return toHash;
        }

        public bool verifyHash(SerializablePackages.Packages.EncryptedPackage message) {
            /*
            SHA512Managed sha = new SHA512Managed();

            byte[] toCompare = sha.ComputeHash(message.encryptedData);
            byte[] originalHash = message.hash;

            if(toCompare.Length == originalHash.Length) {
                for(int x = 0; x != toCompare.Length; x++) {
                    if(toCompare[x] != originalHash[x]) {return false; }
                }
                
            }
            else {
                return false;
            }
            */
            return true;
        }
    }
}