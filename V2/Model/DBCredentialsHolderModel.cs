using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecDBAPI.Model
{
    public class DBCredentialsHolderModel
    {
        public String Status { get; set; }

        public String SealedEncryptedDBAccount { get; set; }

        public String SealedEncryptedDBAccountPassword { get; set; }

        public String DBAccountID { get; set; }
    }
}
