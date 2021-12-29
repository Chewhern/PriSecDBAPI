using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecDBAPI.Model
{
    public class SpecialSelectDBModel
    {
        public SealedDBCredentialModel MyDBCredentialModel { get; set; }

        public String UniquePaymentID { get; set; }

        public String Base64QueryString { get; set; }

        public String[] Base64ParameterName { get; set; }

        public String[] Base64ParameterValue { get; set; }
    }
}
