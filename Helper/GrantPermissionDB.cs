using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;

namespace PriSecDBAPI.Helper
{
    public class GrantPermissionDB
    {
        public MySqlConnection GrantPermissionConnection = new MySqlConnection();
        public Boolean CheckConnection;
        public String SecretPath = "{Path to DB API Grant Account Credentials}";
        public String ConnectionString = "";

        public void setConnection()
        {
            using (StreamReader SecretPathReader = new StreamReader(SecretPath))
            {
                while ((ConnectionString = SecretPathReader.ReadLine()) != null)
                {
                    GrantPermissionConnection.ConnectionString = ConnectionString;
                }
            }
        }

        public Boolean LoadConnection(ref String Exception)
        {
            setConnection();
            try
            {
                GrantPermissionConnection.Open();
                CheckConnection = true;
            }
            catch (MySqlException exception)
            {
                CheckConnection = false;
                Exception = exception.ToString();
            }
            return CheckConnection;
        }

    }
}
