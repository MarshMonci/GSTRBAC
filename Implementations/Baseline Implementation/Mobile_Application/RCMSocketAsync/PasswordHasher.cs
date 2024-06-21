using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace MobileClient
{
    public class PasswordHasher
    {
        public string HashPassword(string password, string username)
        {
            byte[] passwordBytes;
            byte[] hashedBytes;

            //System.IO.StreamWriter newFile = new System.IO.StreamWriter(@"E:\Programming Dump\hashedUserCredentials.txt");
            SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();

            password = password + username;
            passwordBytes = Encoding.ASCII.GetBytes(password);

            hashedBytes = hasher.ComputeHash(passwordBytes);

            return Convert.ToBase64String(hashedBytes);
        }
    }
}
