using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using ASodium;
using System.Runtime.InteropServices;

namespace PriSecDBAPI.Helper
{
    public class CryptographicSecureStrongPasswordGenerator
    {
        public String GenerateUniqueString()
        {
            GCHandle MyGeneralGCHandle = new GCHandle();
            Byte[] CryptographicSecureData = new Byte[240];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(CryptographicSecureData);
            int Loop = 0;
            StringBuilder stringBuilder = new StringBuilder();
            while (Loop < CryptographicSecureData.Length)
            {
                if (CryptographicSecureData[Loop] >= 33 && CryptographicSecureData[Loop] <= 47 && CryptographicSecureData[Loop] != 34 && CryptographicSecureData[Loop] != 39 && CryptographicSecureData[Loop] != 45)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }

                else if (CryptographicSecureData[Loop] >= 48 && CryptographicSecureData[Loop] <= 57)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }
                else if (CryptographicSecureData[Loop] >= 60 && CryptographicSecureData[Loop] <= 63 && CryptographicSecureData[Loop] != 60 && CryptographicSecureData[Loop] != 62)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }
                else if (CryptographicSecureData[Loop] >= 65 && CryptographicSecureData[Loop] <= 90)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }
                else if (CryptographicSecureData[Loop] >= 91 && CryptographicSecureData[Loop] <= 95)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }
                else if (CryptographicSecureData[Loop] >= 97 && CryptographicSecureData[Loop] <= 122)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }
                else if (CryptographicSecureData[Loop] >= 123 && CryptographicSecureData[Loop] <= 126)
                {
                    stringBuilder.Append((char)CryptographicSecureData[Loop]);
                }
                Loop += 1;
            }
            if (stringBuilder.ToString().CompareTo("") != 0)
            {
                MyGeneralGCHandle = GCHandle.Alloc(CryptographicSecureData, GCHandleType.Pinned);
                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CryptographicSecureData.Length);
                MyGeneralGCHandle.Free();
                return stringBuilder.ToString();
            }
            else
            {
                return "";
            }
        }

        public String GenerateMinimumAmountOfUniqueString(int Amount)
        {
            String TestString = GenerateUniqueString();
            while (TestString.Length < Amount)
            {
                TestString += GenerateUniqueString();
            }
            return TestString;
        }
    }
}
