using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;
using ASodium;

namespace PriSecDBAPI.Helper
{
    public class ClientMySQLDBConnection
    {
        //server=localhost;port=3306;database=databasename;user=databaseusername;password=databaseuserpassword;
        public MySqlConnection ClientMySQLConnection = new MySqlConnection();
        public Boolean CheckConnection;
        public String ConnectionString = "server=localhost;port=3306;";

        public void SetConnection(String DBName, String DBUserName, String DBPassword) 
        {

            ConnectionString += "database=" + DBName + ";" + "user=" + DBUserName + ";" + "password=" + DBPassword + ";";

            ClientMySQLConnection.ConnectionString = ConnectionString;
        }

        public Boolean LoadConnection(String DBName, String DBUserName, String DBPassword,ref String Exception)
        {
            SetConnection(DBName,DBUserName,DBPassword);
            try
            {
                ClientMySQLConnection.Open();
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
