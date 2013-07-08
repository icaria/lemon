using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Winterspring.Extensions;

namespace Lemon.Base
{
    public class EncodingException : Exception
    {
        public EncodingException() : base() { }
        public EncodingException(string message) : base(message) { }
    }
    public class DecodingException : Exception
    {
        public DecodingException() : base() { }
        public DecodingException(string message) : base(message) { }
    }

    public class Base32
    {

        private const int BYTE_BITS = 8;
        private const int BASE_32_BITS = 5;
        private static char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

        private static Dictionary<char, int> charValue;
        public static Dictionary<char, int> CharValue
        {
            get
            {
                if (charValue == null)
                {
                    charValue = new Dictionary<char, int>();
                    for (int i = 0; i < alphabet.Length; ++i)
                    {
                        charValue[alphabet[i]] = i;
                    }
                }
                return charValue;
            }
        }

        public static byte[] FromBase32String(string s)
        {
            try
            {
                int i = 0, index = 0, digit = 0;
                int current_char, next_char, next_next_char;
                int bitsUsed;
                List<byte> result = new List<byte>((s.Length + 4) * BASE_32_BITS / BYTE_BITS);

                while (i < s.Length)
                {
                    //The next 8 bits might either span the next 2 characters or the next 3 characters
                    if (index + BYTE_BITS > BASE_32_BITS + BASE_32_BITS)
                    {
                        //Get the next 3 characters
                        //First
                        current_char = CharValue[s[i]];
                        //Second
                        if (i + 1 < s.Length)
                            next_char = CharValue[s[i + 1]];
                        else
                            next_char = 0;
                        //Third
                        if (i + 2 < s.Length)
                            next_next_char = CharValue[s[i + 2]];
                        else
                            next_next_char = 0;

                        //Extract the last bit of first character
                        digit = current_char & (0x1F >> index);
                        digit <<= (BYTE_BITS - (BASE_32_BITS - index));
                        bitsUsed = BASE_32_BITS - index;
                        //Extract second character (the whole thing will be used)
                        digit = digit | next_char << BYTE_BITS - (bitsUsed + BASE_32_BITS);
                        bitsUsed += BASE_32_BITS;
                        //Extract the first bit of the third character
                        digit = digit | next_next_char >> BASE_32_BITS - (BYTE_BITS - bitsUsed);

                        index = (BYTE_BITS - bitsUsed);  //the number we used up in the last character
                        i += 2;
                    }
                    else
                    {
                        //Get the next 2 characters
                        //First
                        current_char = CharValue[s[i]];
                        //Second
                        if (i + 1 < s.Length)
                            next_char = CharValue[s[i + 1]];
                        else
                            next_char = 0;

                        //Extract the last bit of the first character
                        digit = current_char & (0x1F >> index);
                        digit <<= (BYTE_BITS - (BASE_32_BITS - index));
                        bitsUsed = BASE_32_BITS - index;
                        //Extract the first bit of the second character
                        digit = digit | next_char >> BASE_32_BITS - (BYTE_BITS - bitsUsed);

                        index = (BYTE_BITS - bitsUsed);  //the number we used up in the last character
                        ++i;  //We used up at least one character
                        if (index == BASE_32_BITS)
                        {
                            index = 0;
                            ++i;
                        }
                    }
                    result.Add((byte)digit);
                }

                return result.ToArray();
            }
            catch (Exception)
            {
                throw new DecodingException();
            }
        }
        
        //#2652 - Note that you need to set the target length of the base-32 string so that e.g. the last 5 bits of the byte array are
        //0, then it'll handle that properly instead of assuming they're null.
        //However, the byte array must be of the same length (they might need to be padded).  
        //e.g. {0x00, 0x12} will be encoded differently from {0x12}, even if the targetLength is set.
        public static string ToBase32String(byte[] data, int targetLength)
        {
            string s = ToBase32String(data);
            while (s.Length > targetLength)
            {
                if (s[s.Length - 1] == alphabet[0])
                {
                    s = s.Substring(0, s.Length - 1);
                }
                else
                {
                    throw new EncodingException();
                }
            }
            while (s.Length < targetLength)
            {
                s = s + alphabet[0];
            }
            return s;
        }

        public static string ToBase32String(byte[] data)
        {
            int i = 0, index = 0, digit = 0;
            int current_byte, next_byte;
            StringBuilder result = new StringBuilder((data.Length + 7) * BYTE_BITS / BASE_32_BITS);

            while (i < data.Length)
            {
                current_byte = (data[i] >= 0) ? data[i] : (data[i] + 256); // Unsign

                /* Is the current digit going to span a byte boundary? */
                if (index > (BYTE_BITS - BASE_32_BITS))
                {
                    if ((i + 1) < data.Length)
                        next_byte = (data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256);
                    else
                        next_byte = 0;

                    digit = current_byte & (0xFF >> index);
                    index = (index + BASE_32_BITS) % BYTE_BITS;
                    digit <<= index;
                    digit |= next_byte >> (BYTE_BITS - index);
                    i++;
                }
                else
                {
                    digit = (current_byte >> (BYTE_BITS - (index + BASE_32_BITS))) & 0x1F;
                    index = (index + BASE_32_BITS) % BYTE_BITS;
                    if (index == 0)
                        i++;
                }
                try
                {
                    result.Append(alphabet[digit]);
                }
                catch (Exception)
                {
                    throw new EncodingException();
                }
            }

            return result.ToString();
        }

        public static void Test()
        {
            string[] t1 = {
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567",
                "765432ZYXWVUTSRQPONMLKJIHGFEDCBA",
                ",APJPALCOIAF456",
            };
            foreach (string s in t1)
            {
                byte[] b = FromBase32String(s);
                string s2 = ToBase32String(b, s.Length);
                byte[] b2 = FromBase32String(s2);
                string s3 = ToBase32String(b2, s.Length);
                Debug.Assert(ArrayUtil.ArrayEquals<byte>(b, b2));
                Debug.Assert(s == s2);
                Debug.Assert(s == s3);
            }
        }

        public static void Test160BitRandomByte(int count, int bits)
        {
            //Note, this test won't work when bits isn't a multiple of 8
            //because we haven't adjusted the randomly generated bytes
            //to zero out the appropriate number of bits.  e.g. specifying 9 random bits would actually generate 16 random bits

            int byteLength = (bits + 7) / 8;
            int stringLength = (bits + 4) / 5;
            Random r = new Random();
            for (int i = 0; i < count; ++i)
            {
                byte[] b = new byte[byteLength];
                r.NextBytes(b);
                string s1 = ToBase32String(b, (bits+4)/5);
                byte[] b2 = FromBase32String(s1);
                Debug.Assert(ArrayUtil.ArrayEquals<byte>(b, b2));
            }
        }

        public static void Test160BitRandomString(int count, int bits)
        {
            //Note, this test won't work when bits isn't a multiple of 5
            //because we haven't adjusted the randomly generated letters
            //to zero out the appropriate number of bits.  e.g. specifying 6 random bits would actually generate 10 random bits

            int stringLength = (bits + 4) / 5;
            int byteLength = (bits + 7) / 8;
            Random r = new Random();
            for (int numTimesRun = 0; numTimesRun < count; ++numTimesRun)
            {
                StringBuilder sb = new StringBuilder(stringLength);
                for (int i = 0; i < stringLength; ++i)
                {
                    int c = r.Next(0, 32);
                    sb.Append(alphabet[c]);
                }

                string s1 = sb.ToString();
                byte[] b1 = FromBase32String(s1);
                string s2 = ToBase32String(b1, stringLength);
                Debug.Assert(s1 == s2);
            }
        }

        public static bool IsBase32String(string s, int expectedLength)
        {
            //Check that the length is right
            if (expectedLength == 0)
                return string.IsNullOrEmpty(s);
            else if (string.IsNullOrEmpty(s))
                return false;
            if (s.Length != expectedLength) return false;

            return IsBase32String(s);
        }

        public static bool IsBase32String(string s)
        {

            //Check that each character is in the alphabet
            foreach (char c in s)
            {
                foreach (char c2 in alphabet)
                    if (c2 == c)
                        goto Found;
                return false;  //Character not found in alphabet, it's not valid
                Found: continue;                
            }
            return true;
        }
    }
}
