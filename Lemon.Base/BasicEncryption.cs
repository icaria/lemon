using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Org.BouncyCastle.Crypto;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace Lemon.Base
{
    /// <summary>
    /// Some methods for very basic encryption for a database password.
    /// This will be quite easy to break, but just serves as a deterrent.
    /// </summary>
    public class BasicEncryption
    {
        private static byte[] Key = new byte[] {0x35, 0xD6, 0x65, 0xC8, 0xA7, 0x3F, 0xE0, 0xEC, 0x23, 0x71, 0x58, 0xE5, 0x97, 0x99, 0x79, 0xCB, 0xEB, 0x4D, 0xEE, 0xAE, 0xE2, 0xE5, 0x8D, 0x10, 0xE4, 0x8E, 0xDE, 0xF9, 0x7B, 0x47, 0xF7, 0x93};
        
        public static byte[] EncryptData(byte[] plainText)
        {
            RC4Engine eng = new RC4Engine();
            KeyParameter key = new KeyParameter(Key);
            
            //Initialize cipher
            eng.Init(true, key);

            //Do encryption
            byte[] cipherText = new byte[plainText.Length];
            eng.ProcessBytes(plainText, 0, plainText.Length, cipherText, 0);

            return cipherText;
        }

        public static byte[] DecryptData(byte[] cipherText)
        {
            RC4Engine eng = new RC4Engine();
            KeyParameter key = new KeyParameter(Key);

            //Initialize cipher
            eng.Init(true, key);

            //Do encryption
            byte[] plainText = new byte[cipherText.Length];
            eng.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);

            return plainText;
        }

        public static string GenerateRandomPassword()
        {
            //Generate 40 bits of a random password
            byte[] buffer = new byte[5];
            Random r = new Random();
            r.NextBytes(buffer);

            return Base32.ToBase32String(buffer, 8);
        }

        public static byte[] EncryptPasswordBinary(string plainText)
        {
            //Convert the text into bytes in UTF8, encrypt that, and return the encrypted password.
            return EncryptData(Encoding.UTF8.GetBytes(plainText));
        }

        public static string DecryptPasswordBinary(byte[] cipherText)
        {
            //Decrypt the cipherText and then encode it using UTF8 into a string
            return Encoding.UTF8.GetString(DecryptData(cipherText));
        }

    }
}
