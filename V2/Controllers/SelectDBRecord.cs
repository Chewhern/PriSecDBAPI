using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASodium;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using PriSecDBAPI.Model;
using PriSecDBAPI.Helper;

namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectDBRecord : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpPost]
        public DBRecordsModel RetrieveDBRecords(NormalDBModel MySelectModel) 
        {
            DBRecordsModel MyRecordsModel = new DBRecordsModel();
            String Status = "";
            String[] RetrievedRecords = new String[] { };
            ClientMySQLDBConnection ClientMySQLDB = new ClientMySQLDBConnection();
            String Path = "{Path to Sealed Session}";
            Byte[] CipheredDBNameByte = new Byte[] { };
            Byte[] DBNameByte = new Byte[] { };
            String DBName = "";
            Byte[] CipheredDBUserNameByte = new Byte[] { };
            Byte[] DBUserNameByte = new Byte[] { };
            String DBUserName = "";
            Byte[] CipheredDBUserPasswordByte = new Byte[] { };
            Byte[] DBUserPasswordByte = new Byte[] { };
            String DBUserPassword = "";
            Byte[] ServerSealedDHPrivateKeyByte = new Byte[] { };
            Byte[] ServerSealedDHPublicKeyByte = new Byte[] { };
            int DBCount = 0;
            int LoopCount = 0;
            int ArrayCount = 0;
            int PaymentIDCount = 0;
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            MySqlCommand ClientMySQLGeneralQuery = new MySqlCommand();
            String ClientExceptionString = "";
            Boolean ClientCheckConnection = true;
            MySqlDataReader ClientDataReader;
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DatabaseExpirationTime = new DateTime();
            Byte[] QueryStringByte = new Byte[] { };
            String QueryString = "";
            Byte[] ParameterNameByte = new Byte[] { };
            String ParameterName = "";
            Byte[] ParameterValueByte = new Byte[] { };
            String ParameterValue = "";
            if (MySelectModel != null) 
            {
                if (MySelectModel.MyDBCredentialModel.SealedSessionID != null)
                {
                    Path += MySelectModel.MyDBCredentialModel.SealedSessionID;
                    if (Directory.Exists(Path) == true) 
                    {
                        try
                        {
                            ServerSealedDHPrivateKeyByte = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                        }
                        catch
                        {
                            Status = "Error: Can't find server DH private key";
                            MyRecordsModel.Status = Status;
                            MyRecordsModel.ParameterValues = null;
                            return MyRecordsModel;
                        }
                        try
                        {
                            CipheredDBNameByte = Convert.FromBase64String(MySelectModel.MyDBCredentialModel.SealedDBName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Name is not in base 64 format";
                            MyRecordsModel.Status = Status;
                            MyRecordsModel.ParameterValues = null;
                            return MyRecordsModel;
                        }
                        try
                        {
                            CipheredDBUserNameByte = Convert.FromBase64String(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB User Name is not in base 64 format";
                            MyRecordsModel.Status = Status;
                            MyRecordsModel.ParameterValues = null;
                            return MyRecordsModel;
                        }
                        try
                        {
                            CipheredDBUserPasswordByte = Convert.FromBase64String(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Password is not in base 64 format";
                            MyRecordsModel.Status = Status;
                            MyRecordsModel.ParameterValues = null;
                            return MyRecordsModel;
                        }
                        ServerSealedDHPublicKeyByte = SodiumScalarMult.Base(ServerSealedDHPrivateKeyByte);
                        try
                        {
                            DBNameByte = SodiumSealedPublicKeyBox.Open(CipheredDBNameByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte);
                            DBUserNameByte = SodiumSealedPublicKeyBox.Open(CipheredDBUserNameByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte);
                            DBUserPasswordByte = SodiumSealedPublicKeyBox.Open(CipheredDBUserPasswordByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte,true);
                        }
                        catch
                        {
                            Status = "Error: Unable to decrypt sealed DB credentials";
                            MyRecordsModel.Status = Status;
                            MyRecordsModel.ParameterValues = null;
                            return MyRecordsModel;
                        }
                        DBName = Encoding.UTF8.GetString(DBNameByte);
                        DBUserName = Encoding.UTF8.GetString(DBUserNameByte);
                        DBUserPassword = Encoding.UTF8.GetString(DBUserPasswordByte);
                        SodiumSecureMemory.SecureClearBytes(ServerSealedDHPublicKeyByte);
                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Payment` WHERE `ID`=@ID";
                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySelectModel.UniquePaymentID;
                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                        MySQLGeneralQuery.Prepare();
                        PaymentIDCount = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                        if (PaymentIDCount == 1) 
                        {
                            ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                            ClientCheckConnection = ClientMySQLDB.CheckConnection;
                            if (ClientCheckConnection == true) 
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "SELECT `Expiration_Date` FROM `Payment` WHERE `ID`=@ID";
                                MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySelectModel.UniquePaymentID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                DatabaseExpirationTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (DateTime.Compare(MyUTC8DateTime, DatabaseExpirationTime) <= 0)
                                {
                                    try
                                    {
                                        QueryStringByte = Convert.FromBase64String(MySelectModel.Base64QueryString);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: The query must be in base64 format";
                                        MyRecordsModel.Status = Status;
                                        MyRecordsModel.ParameterValues = null;
                                        return MyRecordsModel;
                                    }
                                    try
                                    {
                                        ParameterNameByte = Convert.FromBase64String(MySelectModel.Base64ParameterName);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: The parameter name must be in base64 format";
                                        MyRecordsModel.Status = Status;
                                        MyRecordsModel.ParameterValues = null;
                                        return MyRecordsModel;
                                    }
                                    try
                                    {
                                        ParameterValueByte = Convert.FromBase64String(MySelectModel.Base64ParameterValue);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: The parameter value must be in base64 format";
                                        MyRecordsModel.Status = Status;
                                        MyRecordsModel.ParameterValues = null;
                                        return MyRecordsModel;
                                    }
                                    QueryString = Encoding.UTF8.GetString(QueryStringByte);
                                    if (QueryString.ToLower().Contains("select") == false || (QueryString.Contains("'") == true || QueryString.ToLower().Contains("update") == true || QueryString.ToLower().Contains("delete") == true || QueryString.ToLower().Contains("insert") == true || QueryString.ToLower().Contains(";") == true))
                                    {
                                        Status = "Error: You didn't pass in correct select query";
                                        MyRecordsModel.Status = Status;
                                        MyRecordsModel.ParameterValues = null;
                                        return MyRecordsModel;
                                    }
                                    ParameterName = Encoding.UTF8.GetString(ParameterNameByte);
                                    ParameterValue = Encoding.UTF8.GetString(ParameterValueByte);
                                    ClientMySQLGeneralQuery.CommandText = QueryString;
                                    ClientMySQLGeneralQuery.Parameters.Add("@" + ParameterName, MySqlDbType.Text).Value = ParameterValue;
                                    ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                    ClientMySQLGeneralQuery.Prepare();
                                    ClientDataReader = ClientMySQLGeneralQuery.ExecuteReader();
                                    //Potential unsolvable issue which could lead to DDOS attack
                                    //The code here involves with both counting row count and retrieving records(if exists)
                                    //Ideally, in all mysql databases, this is not an issue as the database and the server were
                                    //used by only certain types of users
                                    //through the use of LIMIT and OFFSET key words in MySQL
                                    //The developer's MySQL query can also be treated as nuclear wastes
                                    //in such cases, if they don't use the keywords of LIMIT and OFFSET
                                    //it could be used intentionally or unintentionally as a way to overburden the server.
                                    //The likelihood that it can happen is unknown.. Please do take note of these comments.
                                    while (ClientDataReader.Read())
                                    {
                                        DBCount += 1;
                                    }
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                    ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                    ClientMySQLGeneralQuery = new MySqlCommand();
                                    ClientMySQLGeneralQuery.CommandText = QueryString;
                                    ClientMySQLGeneralQuery.Parameters.Add("@" + ParameterName, MySqlDbType.Text).Value = ParameterValue;
                                    ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                    ClientMySQLGeneralQuery.Prepare();
                                    ClientDataReader = ClientMySQLGeneralQuery.ExecuteReader();
                                    if (DBCount != 0) 
                                    {
                                        DBCount *= ClientDataReader.FieldCount;
                                        RetrievedRecords = new String[DBCount];
                                        while (ClientDataReader.Read()) 
                                        {
                                            while (LoopCount<ClientDataReader.FieldCount && ArrayCount<RetrievedRecords.Length) 
                                            {
                                                RetrievedRecords[ArrayCount] = ClientDataReader.GetValue(LoopCount).ToString();
                                                LoopCount += 1;
                                                ArrayCount += 1;
                                            }
                                            LoopCount = 0;
                                        }
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Successed: Records have been retrieved from specified database table";
                                        MyRecordsModel.Status = Status;
                                        MyRecordsModel.ParameterValues = RetrievedRecords;
                                        return MyRecordsModel;
                                    }
                                    else 
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: The specified database in your table has no records at all";
                                        MyRecordsModel.Status = Status;
                                        MyRecordsModel.ParameterValues = null;
                                        return MyRecordsModel;
                                    }
                                }
                                else 
                                {
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                    SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                    SodiumSecureMemory.SecureClearString(DBName);
                                    SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                    SodiumSecureMemory.SecureClearString(DBUserName);
                                    SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                    SodiumSecureMemory.SecureClearString(DBUserPassword);
                                    SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                    Status = "Error: You haven't renew the database";
                                    MyRecordsModel.Status = Status;
                                    MyRecordsModel.ParameterValues = null;
                                    return MyRecordsModel;
                                }
                            }
                            else 
                            {
                                ClientMySQLDB.ClientMySQLConnection.Close();
                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                SodiumSecureMemory.SecureClearString(DBName);
                                SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                SodiumSecureMemory.SecureClearString(DBUserName);
                                SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                                Status = "Error: You have input wrong db credentials";
                                MyRecordsModel.Status = Status;
                                MyRecordsModel.ParameterValues = null;
                                return MyRecordsModel;
                            }
                        }
                        else 
                        {
                            ClientMySQLDB.ClientMySQLConnection.Close();
                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                            SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBName);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                            SodiumSecureMemory.SecureClearString(DBName);
                            SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserName);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                            SodiumSecureMemory.SecureClearString(DBUserName);
                            SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedDBUserPassword);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                            SodiumSecureMemory.SecureClearString(MySelectModel.MyDBCredentialModel.SealedSessionID);
                            Status = "Error: The sent payment ID does not exists";
                            MyRecordsModel.Status = Status;
                            MyRecordsModel.ParameterValues = null;
                            return MyRecordsModel;
                        }
                    }
                    else 
                    {
                        Status = "Error: The sealed session ID does not exist";
                        MyRecordsModel.Status = Status;
                        MyRecordsModel.ParameterValues = null;
                        return MyRecordsModel;
                    }
                }
                else 
                {
                    Status = "Error: The sealed session ID can't be null";
                    MyRecordsModel.Status = Status;
                    MyRecordsModel.ParameterValues = null;
                    return MyRecordsModel;
                }
            }
            else 
            {
                Status = "Error: NormalDBModel can't be null";
                MyRecordsModel.Status = Status;
                MyRecordsModel.ParameterValues = null;
                return MyRecordsModel;
            }
        }
    }
}
