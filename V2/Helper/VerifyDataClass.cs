using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASodium;

namespace PriSecDBAPI.Helper
{
    public class VerifyDataClass
    {
        public void VerifyData(ref Boolean ReferenceBoolean, ref Byte[] VerifiedData, Byte[] SignedData, Byte[] ED25519PK)
        {
            try
            {
                VerifiedData = SodiumPublicKeyAuth.Verify(SignedData, ED25519PK);
                ReferenceBoolean = true;
            }
            catch
            {
                ReferenceBoolean = false;
            }
        }
    }
}
