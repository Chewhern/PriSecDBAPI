using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecDBAPI.Model
{
    public class PaymentModel
    {
        public String Status { get; set; }

        public String CipheredDBName { get; set; }

        public String CipheredDBAccountUserName { get; set; }

        public String CipheredDBAccountPassword { get; set; }

        public String SystemPaymentID { get; set; }

        public String GMT8PaymentMadeDateTime { get; set; }
    }
}
