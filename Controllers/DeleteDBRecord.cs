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
using System.Runtime.InteropServices;
using PriSecDBAPI.Model;
using PriSecDBAPI.Helper;

namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteDBRecord : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpPost]
        public String DeleteDBData(NormalDBModel MyDeleteModel) 
        {
            String Status = "";
            ClientMySQLDBConnection ClientMySQLDB = new ClientMySQLDBConnection();
            String Path = "{Path to Sealed Session folder}";
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
            int PaymentIDCount = 0;
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            MySqlCommand ClientMySQLGeneralQuery = new MySqlCommand();
            MySqlCommand ClientSQLQuery = new MySqlCommand();
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
            GCHandle MyGeneralGCHandle = new GCHandle();
            int SQLLoopCount = 1;
            String UsedAmountString = null;
            if (MyDeleteModel != null)
            {
                if (MyDeleteModel.MyDBCredentialModel.SealedSessionID != null)
                {
                    Path += MyDeleteModel.MyDBCredentialModel.SealedSessionID;
                    if (Directory.Exists(Path) == true)
                    {
                        try
                        {
                            ServerSealedDHPrivateKeyByte = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                        }
                        catch
                        {
                            Status = "Error: Can't find server DH private key";
                            return Status;
                        }
                        try
                        {
                            CipheredDBNameByte = Convert.FromBase64String(MyDeleteModel.MyDBCredentialModel.SealedDBName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Name is not in base 64 format";
                            return Status;
                        }
                        try
                        {
                            CipheredDBUserNameByte = Convert.FromBase64String(MyDeleteModel.MyDBCredentialModel.SealedDBUserName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB User Name is not in base 64 format";
                            return Status;
                        }
                        try
                        {
                            CipheredDBUserPasswordByte = Convert.FromBase64String(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Password is not in base 64 format";
                            return Status;
                        }
                        ServerSealedDHPublicKeyByte = SodiumScalarMult.Base(ServerSealedDHPrivateKeyByte);
                        try
                        {
                            ServerSealedDHPrivateKeyByte = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                            DBNameByte = SodiumSealedPublicKeyBox.Open(CipheredDBNameByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte);
                            ServerSealedDHPrivateKeyByte = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                            DBUserNameByte = SodiumSealedPublicKeyBox.Open(CipheredDBUserNameByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte);
                            ServerSealedDHPrivateKeyByte = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                            DBUserPasswordByte = SodiumSealedPublicKeyBox.Open(CipheredDBUserPasswordByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte);
                        }
                        catch
                        {
                            Status = "Error: Unable to decrypt sealed DB credentials";
                            return Status;
                        }
                        DBName = Encoding.UTF8.GetString(DBNameByte);
                        DBUserName = Encoding.UTF8.GetString(DBUserNameByte);
                        DBUserPassword = Encoding.UTF8.GetString(DBUserPasswordByte);
                        MyGeneralGCHandle = GCHandle.Alloc(ServerSealedDHPrivateKeyByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ServerSealedDHPrivateKeyByte.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(ServerSealedDHPublicKeyByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ServerSealedDHPublicKeyByte.Length);
                        MyGeneralGCHandle.Free();
                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Payment` WHERE `ID`=@ID";
                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyDeleteModel.UniquePaymentID;
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
                                MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyDeleteModel.UniquePaymentID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                DatabaseExpirationTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (DateTime.Compare(MyUTC8DateTime, DatabaseExpirationTime) <= 0)
                                {
                                    try
                                    {
                                        QueryStringByte = Convert.FromBase64String(MyDeleteModel.Base64QueryString);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                                        MyGeneralGCHandle.Free();
                                        Status = "Error: The query must be in base64 format";
                                        return Status;
                                    }
                                    try
                                    {
                                        ParameterNameByte = Convert.FromBase64String(MyDeleteModel.Base64ParameterName);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                                        MyGeneralGCHandle.Free();
                                        Status = "Error: The parameter name must be in base64 format";
                                        return Status;
                                    }
                                    try
                                    {
                                        ParameterValueByte = Convert.FromBase64String(MyDeleteModel.Base64ParameterValue);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                                        MyGeneralGCHandle.Free();
                                        Status = "Error: The parameter value must be in base64 format";
                                        return Status;
                                    }
                                    QueryString = Encoding.UTF8.GetString(QueryStringByte);
                                    if (QueryString.ToLower().Contains("delete") == false || QueryString.ToLower().Contains("update") == true || QueryString.ToLower().Contains("select") == true || QueryString.ToLower().Contains("insert") == true || QueryString.ToLower().Contains(";") == true) 
                                    {
                                        Status = "Error: You didn't pass in a delete query string";
                                        return Status;
                                    }
                                    ParameterName = Encoding.UTF8.GetString(ParameterNameByte);
                                    ParameterValue = Encoding.UTF8.GetString(ParameterValueByte);
                                    ClientMySQLGeneralQuery.CommandText = QueryString;
                                    ClientMySQLGeneralQuery.Parameters.Add("@" + ParameterName, MySqlDbType.Text).Value = ParameterValue;
                                    ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                    ClientMySQLGeneralQuery.Prepare();
                                    ClientMySQLGeneralQuery.ExecuteNonQuery();
                                    ClientSQLQuery.CommandText = "SELECT table_schema 'Databases',ROUND(SUM(data_length + index_length), 1) AS 'Size In Bytes' FROM information_schema.tables GROUP BY table_schema";
                                    ClientSQLQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                    ClientSQLQuery.Prepare();
                                    ClientDataReader = ClientSQLQuery.ExecuteReader();
                                    while (ClientDataReader.Read())
                                    {
                                        if (SQLLoopCount == 2)
                                        {
                                            UsedAmountString = ClientDataReader.GetValue(1).ToString();
                                        }
                                        SQLLoopCount += 1;
                                    }
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                                    MyGeneralGCHandle.Free();
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "UPDATE `Payment` SET `Bytes_Used`=@Bytes_Used WHERE `ID`=@ID";
                                    MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyDeleteModel.UniquePaymentID;
                                    MySQLGeneralQuery.Parameters.Add("@Bytes_Used", MySqlDbType.Text).Value = UsedAmountString;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    MySQLGeneralQuery.ExecuteNonQuery();
                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                    Status = "Successed: The specified data row has been deleted";
                                    return Status;
                                }
                                else
                                {
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                                    MyGeneralGCHandle.Free();
                                    Status = "Error: You haven't renew the database";
                                    return Status;
                                }
                            }
                            else
                            {
                                ClientMySQLDB.ClientMySQLConnection.Close();
                                MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                                MyGeneralGCHandle.Free();
                                Status = "Error: You have input wrong db credentials";
                                return Status;
                            }
                        }
                        else
                        {
                            ClientMySQLDB.ClientMySQLConnection.Close();
                            MyGeneralGCHandle = GCHandle.Alloc(ClientMySQLDB.ClientMySQLConnection.ConnectionString, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientMySQLDB.ClientMySQLConnection.ConnectionString.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBName, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBName.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(CipheredDBNameByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBNameByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DBNameByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBNameByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DBName, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBName.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserName, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserName.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserNameByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserNameByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DBUserNameByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserNameByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DBUserName, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserName.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedDBUserPassword.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(CipheredDBUserPasswordByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDBUserPasswordByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DBUserPasswordByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPasswordByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DBUserPassword, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DBUserPassword.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(MyDeleteModel.MyDBCredentialModel.SealedSessionID, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), MyDeleteModel.MyDBCredentialModel.SealedSessionID.Length);
                            MyGeneralGCHandle.Free();
                            Status = "Error: The sent payment ID does not exists";
                            return Status;
                        }
                    }
                    else
                    {
                        Status = "Error: The sealed session ID does not exist";
                        return Status;
                    }
                }
                else
                {
                    Status = "Error: The sealed session ID can't be null";
                    return Status;
                }
            }
            else
            {
                Status = "Error: NormalDBModel can't be null";
                return Status;
            }
        }
    }
}
