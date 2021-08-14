using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecDBAPI.Model
{
    public class SealedDBCredentialModel
    {
        public String SealedSessionID { get; set; }

        public String SealedDBName { get; set; }

        public String SealedDBUserName { get; set; }

        public String SealedDBUserPassword { get; set; }
    }
}
