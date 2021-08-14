using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecDBAPI.Helper
{
    public class ConvertFromBase64StringClass
    {
        public void ConvertFromBase64StringFunction(ref Boolean ReferenceBoolean, ref Byte[] ReferenceByte, String Base64String)
        {
            try
            {
                ReferenceByte = Convert.FromBase64String(Base64String);
                ReferenceBoolean = true;
            }
            catch
            {
                ReferenceBoolean = false;
            }
        }
    }
}
