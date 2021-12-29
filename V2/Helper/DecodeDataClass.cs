using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PriSecDBAPI.Helper
{
    public class DecodeDataClass
    {
        public void DecodeDataFunction(ref Boolean ReferenceBoolean, ref String ReferenceDataString, String DataString)
        {
            try
            {
                if (DataString.Contains("+"))
                {
                    ReferenceDataString = DataString;
                }
                else
                {
                    ReferenceDataString = HttpUtility.UrlDecode(DataString);
                }
                ReferenceBoolean = true;
            }
            catch
            {
                ReferenceBoolean = false;
            }
        }
    }
}
