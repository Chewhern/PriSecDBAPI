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
    public class SpecialSealedX3DHDBUpdate : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();
        private CryptographicSecureIDGenerator MyIDGenerator = new CryptographicSecureIDGenerator();

        [HttpPost]
        public String UpdateDBRecords(SpecialDBModel MySpecialDBModel)
        {
            ClientMySQLDBConnection ClientMySQLDB = new ClientMySQLDBConnection();
            String Status = "";
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
            int PaymentIDCount = 0;
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            MySqlDataReader DataReader;
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
            Byte[] EncryptedParameterValueByte = new Byte[] { };
            Byte[] SignedSPKX25519PKByte = new Byte[] { };
            Byte[] SPKED25519PKByte = new Byte[] { };
            Byte[] SPKX25519PKByte = new Byte[] { };
            Byte[] SignedIKX25519PKByte = new Byte[] { };
            Byte[] IKED25519PKByte = new Byte[] { };
            Byte[] IKX25519PKByte = new Byte[] { };
            Byte[] SignedOPKX25519PKByte = new Byte[] { };
            Byte[] OPKED25519PKByte = new Byte[] { };
            Byte[] OPKX25519PKByte = new Byte[] { };
            Byte[] SharedSecret1 = new Byte[] { };
            Byte[] SharedSecret2 = new Byte[] { };
            Byte[] SharedSecret3 = new Byte[] { };
            Byte[] SharedSecret4 = new Byte[] { };
            Byte[] ConcatedSharedSecret = new Byte[] { };
            Byte[] MasterSharedSecret = new Byte[] { };
            Byte[] AliceConcatedX25519PKByte = new Byte[] { };
            Byte[] BobConcatedX25519PKByte = new Byte[] { };
            Byte[] AliceCheckSum = new Byte[] { };
            Byte[] BobCheckSum = new Byte[] { };
            Byte[] ConcatedCheckSum = new Byte[] { };
            Byte[] Nonce = new Byte[] { };
            String ID = MyIDGenerator.GenerateUniqueString();
            if (ID.Length > 16)
            {
                ID = ID.Substring(0, 16);
            }
            int LoopCount = 0;
            String UsedAmountString = null;
            ulong DBUsedAmount = 0;
            ulong NewDBUsedAmount = 0;
            String SuccessStatus = "";
            int TablesThatHasPKCount = 0;
            String[] TablesNameThatHasPK = new String[] { };
            String[] TablesPKColumnName = new String[] { };
            Boolean[] IsPKFieldData = new Boolean[] { };
            int SQLLoopCount = 0;
            int SQLCount = 0;
            Boolean CheckIDValueArray = true;
            if (MySpecialDBModel != null)
            {
                if (MySpecialDBModel.MyDBCredentialModel.SealedSessionID != null)
                {
                    Path += MySpecialDBModel.MyDBCredentialModel.SealedSessionID;
                    IsPKFieldData = new Boolean[MySpecialDBModel.IDValue.Length];
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
                            CipheredDBNameByte = Convert.FromBase64String(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Name is not in base 64 format";
                            return Status;
                        }
                        try
                        {
                            CipheredDBUserNameByte = Convert.FromBase64String(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB User Name is not in base 64 format";
                            return Status;
                        }
                        try
                        {
                            CipheredDBUserPasswordByte = Convert.FromBase64String(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Password is not in base 64 format";
                            return Status;
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
                            return Status;
                        }
                        DBName = Encoding.UTF8.GetString(DBNameByte);
                        DBUserName = Encoding.UTF8.GetString(DBUserNameByte);
                        DBUserPassword = Encoding.UTF8.GetString(DBUserPasswordByte);
                        SodiumSecureMemory.SecureClearBytes(ServerSealedDHPublicKeyByte);
                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Payment` WHERE `ID`=@ID";
                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySpecialDBModel.UniquePaymentID;
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
                                MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySpecialDBModel.UniquePaymentID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                DatabaseExpirationTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (DateTime.Compare(MyUTC8DateTime, DatabaseExpirationTime) <= 0)
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT * FROM `X3SDH` WHERE `Payment_ID`=@Payment_ID";
                                    MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = MySpecialDBModel.UniquePaymentID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    DataReader = MySQLGeneralQuery.ExecuteReader();
                                    while (DataReader.Read())
                                    {
                                        SignedSPKX25519PKByte = Convert.FromBase64String(DataReader.GetValue(1).ToString());
                                        SPKED25519PKByte = Convert.FromBase64String(DataReader.GetValue(2).ToString());
                                        SignedIKX25519PKByte = Convert.FromBase64String(DataReader.GetValue(3).ToString());
                                        IKED25519PKByte = Convert.FromBase64String(DataReader.GetValue(4).ToString());
                                        SignedOPKX25519PKByte = Convert.FromBase64String(DataReader.GetValue(5).ToString());
                                        OPKED25519PKByte = Convert.FromBase64String(DataReader.GetValue(6).ToString());
                                    }
                                    SPKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSPKX25519PKByte, SPKED25519PKByte);
                                    IKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedIKX25519PKByte, IKED25519PKByte);
                                    OPKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedOPKX25519PKByte, OPKED25519PKByte);
                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                    myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                    //Get the signed X25519 public key
                                    try
                                    {
                                        QueryStringByte = Convert.FromBase64String(MySpecialDBModel.Base64QueryString);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: The query must be in base64 format";
                                        return Status;
                                    }
                                    QueryString = Encoding.UTF8.GetString(QueryStringByte);
                                    if (QueryString.ToLower().Contains("update") == false || (QueryString.ToLower().Contains("insert") == true || QueryString.ToLower().Contains("delete") == true || QueryString.ToLower().Contains("select") == true || QueryString.ToLower().Contains(";") == true || QueryString.ToLower().Contains("'") == true))
                                    {
                                        Status = "Error: You didn't pass in an update query string";
                                        return Status;
                                    }
                                    ClientMySQLGeneralQuery.CommandText = "select tab.table_schema as database_schema,sta.index_name as pk_name,sta.seq_in_index as column_id,sta.column_name,tab.table_name from information_schema.tables as tab inner join information_schema.statistics as sta on sta.table_schema = tab.table_schema and sta.table_name = tab.table_name and sta.index_name = 'primary' where tab.table_schema = '" + DBName + "' and tab.table_type = 'BASE TABLE' order by tab.table_name,column_id;";
                                    ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                    ClientMySQLGeneralQuery.Prepare();
                                    ClientDataReader = ClientMySQLGeneralQuery.ExecuteReader();
                                    while (ClientDataReader.Read())
                                    {
                                        TablesThatHasPKCount += 1;
                                    }
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                    ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                    TablesPKColumnName = new String[TablesThatHasPKCount];
                                    TablesNameThatHasPK = new string[TablesThatHasPKCount];
                                    ClientMySQLGeneralQuery = new MySqlCommand();
                                    ClientMySQLGeneralQuery.CommandText = "select tab.table_schema as database_schema,sta.index_name as pk_name,sta.seq_in_index as column_id,sta.column_name,tab.table_name from information_schema.tables as tab inner join information_schema.statistics as sta on sta.table_schema = tab.table_schema and sta.table_name = tab.table_name and sta.index_name = 'primary' where tab.table_schema = '" + DBName + "' and tab.table_type = 'BASE TABLE' order by tab.table_name,column_id;";
                                    ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                    ClientMySQLGeneralQuery.Prepare();
                                    ClientDataReader = ClientMySQLGeneralQuery.ExecuteReader();
                                    while (ClientDataReader.Read())
                                    {
                                        TablesPKColumnName[SQLLoopCount] = ClientDataReader.GetValue(3).ToString();
                                        TablesNameThatHasPK[SQLLoopCount] = ClientDataReader.GetValue(4).ToString();
                                        SQLLoopCount += 1;
                                    }
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                    ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                    ClientMySQLGeneralQuery = new MySqlCommand();
                                    if (MySpecialDBModel.IDValue.Length != 0)
                                    {
                                        if (TablesThatHasPKCount != 0)
                                        {
                                            SQLLoopCount = 0;
                                            while (LoopCount < MySpecialDBModel.IDValue.Length)
                                            {
                                                while (SQLLoopCount < TablesNameThatHasPK.Length)
                                                {
                                                    ClientMySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `" + TablesNameThatHasPK[SQLLoopCount] + "` WHERE `" + TablesPKColumnName[SQLLoopCount] + "`=@" + TablesPKColumnName[SQLLoopCount];
                                                    ClientMySQLGeneralQuery.Parameters.Add("@" + TablesPKColumnName[SQLLoopCount], MySqlDbType.Text).Value = MySpecialDBModel.IDValue[LoopCount];
                                                    ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                                    ClientMySQLGeneralQuery.Prepare();
                                                    SQLCount = int.Parse(ClientMySQLGeneralQuery.ExecuteScalar().ToString());
                                                    ClientMySQLGeneralQuery = new MySqlCommand();
                                                    if (SQLCount == 1)
                                                    {
                                                        IsPKFieldData[LoopCount] = true;
                                                    }
                                                    SQLLoopCount += 1;
                                                }
                                                SQLLoopCount = 0;
                                                LoopCount += 1;
                                            }
                                            LoopCount = 0;
                                            while (LoopCount < IsPKFieldData.Length)
                                            {
                                                if (IsPKFieldData[LoopCount] == false)
                                                {
                                                    CheckIDValueArray = false;
                                                    break;
                                                }
                                                LoopCount += 1;
                                            }
                                            LoopCount = 0;
                                            if (CheckIDValueArray == true)
                                            {
                                                ClientMySQLGeneralQuery.CommandText = QueryString;
                                                if (MySpecialDBModel.Base64ParameterName.Length != 0 && MySpecialDBModel.Base64ParameterValue.Length != 0)
                                                {
                                                    if (MySpecialDBModel.Base64ParameterName.Length == MySpecialDBModel.Base64ParameterValue.Length)
                                                    {
                                                        while (LoopCount < MySpecialDBModel.Base64ParameterName.Length)
                                                        {
                                                            RevampedKeyPair AliceIdentityKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                                                            RevampedKeyPair AliceEphemeralKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                                                            ParameterNameByte = Convert.FromBase64String(MySpecialDBModel.Base64ParameterName[LoopCount]);
                                                            ParameterName = Encoding.UTF8.GetString(ParameterNameByte);
                                                            ParameterValueByte = Convert.FromBase64String(MySpecialDBModel.Base64ParameterValue[LoopCount]);
                                                            SharedSecret1 = SodiumScalarMult.Mult(AliceIdentityKeyPair.PrivateKey, SPKX25519PKByte,true);
                                                            SharedSecret2 = SodiumScalarMult.Mult(AliceEphemeralKeyPair.PrivateKey, IKX25519PKByte,true);
                                                            SharedSecret3 = SodiumScalarMult.Mult(AliceEphemeralKeyPair.PrivateKey, SPKX25519PKByte,true);
                                                            SharedSecret4 = SodiumScalarMult.Mult(AliceEphemeralKeyPair.PrivateKey, OPKX25519PKByte,true);
                                                            ConcatedSharedSecret = SharedSecret1.Concat(SharedSecret2).Concat(SharedSecret3).Concat(SharedSecret4).ToArray();
                                                            MasterSharedSecret = SodiumKDF.KDFFunction(32, 1, "X3DHSKey", ConcatedSharedSecret,true);
                                                            AliceConcatedX25519PKByte = AliceIdentityKeyPair.PublicKey.Concat(AliceEphemeralKeyPair.PublicKey).ToArray();
                                                            BobConcatedX25519PKByte = SPKX25519PKByte.Concat(IKX25519PKByte).Concat(OPKX25519PKByte).ToArray();
                                                            AliceCheckSum = SodiumGenericHash.ComputeHash(64, AliceConcatedX25519PKByte);
                                                            BobCheckSum = SodiumGenericHash.ComputeHash(64, BobConcatedX25519PKByte);
                                                            ConcatedCheckSum = AliceCheckSum.Concat(BobCheckSum).ToArray();
                                                            if (MySpecialDBModel.IsXSalsa20Poly1305 == true)
                                                            {
                                                                Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumSecretBox.GenerateNonce().Length, ConcatedCheckSum);
                                                                EncryptedParameterValueByte = SodiumSecretBox.Create(ParameterValueByte, Nonce, MasterSharedSecret,true);
                                                            }
                                                            else
                                                            {
                                                                Nonce = SodiumGenericHash.ComputeHash((Byte)SodiumSecretBoxXChaCha20Poly1305.GenerateNonce().Length, ConcatedCheckSum);
                                                                EncryptedParameterValueByte = SodiumSecretBoxXChaCha20Poly1305.Create(ParameterValueByte, Nonce, MasterSharedSecret,true);
                                                            }
                                                            EncryptedParameterValueByte = AliceIdentityKeyPair.PublicKey.Concat(AliceEphemeralKeyPair.PublicKey).Concat(EncryptedParameterValueByte).ToArray();
                                                            NewDBUsedAmount += (ulong)EncryptedParameterValueByte.Length;
                                                            ClientMySQLGeneralQuery.Parameters.Add("@" + ParameterName, MySqlDbType.Text).Value = Convert.ToBase64String(EncryptedParameterValueByte);
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret1);
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret2);
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret3);
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret4);
                                                            AliceIdentityKeyPair.Clear();
                                                            AliceEphemeralKeyPair.Clear();
                                                            LoopCount += 1;
                                                        }
                                                        LoopCount = 0;
                                                        while (LoopCount < MySpecialDBModel.IDValue.Length)
                                                        {
                                                            ClientMySQLGeneralQuery.Parameters.Add("@Update_ID" + LoopCount.ToString(), MySqlDbType.Text).Value = MySpecialDBModel.IDValue[LoopCount];
                                                            NewDBUsedAmount += (ulong)MySpecialDBModel.IDValue[LoopCount].Length;
                                                            LoopCount += 1;
                                                        }
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
                                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString); 
                                                        ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                                        if (UsedAmountString != null && UsedAmountString.CompareTo("") != 0)
                                                        {
                                                            DBUsedAmount = ulong.Parse(UsedAmountString);
                                                            //2 GB
                                                            if (DBUsedAmount > 2147483648)
                                                            {
                                                                ClientMySQLDB.ClientMySQLConnection.Close();
                                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                                Status = "Error: Please delete some data in your database, you have used more than 2GB";
                                                                return Status;
                                                            }
                                                        }
                                                        ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                                        ClientMySQLGeneralQuery.Prepare();
                                                        ClientMySQLGeneralQuery.ExecuteNonQuery();
                                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                        MySQLGeneralQuery = new MySqlCommand();
                                                        MySQLGeneralQuery.CommandText = "UPDATE `Payment` SET `Bytes_Used`=@Bytes_Used WHERE `ID`=@ID";
                                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySpecialDBModel.UniquePaymentID;
                                                        MySQLGeneralQuery.Parameters.Add("@Bytes_Used", MySqlDbType.Text).Value = DBUsedAmount.ToString();
                                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                        MySQLGeneralQuery.Prepare();
                                                        MySQLGeneralQuery.ExecuteNonQuery();
                                                        myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                        SuccessStatus = "Records have been updated with numerous ID Values array that has been supplied by user";
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                        SodiumSecureMemory.SecureClearString(DBName);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                        return SuccessStatus;
                                                    }
                                                    else
                                                    {
                                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                        SodiumSecureMemory.SecureClearString(DBName);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                        Status = "Error: Parameter name array length and parameter value array length is not the same";
                                                        return Status;
                                                    }
                                                }
                                                else
                                                {
                                                    if (MySpecialDBModel.IDValue.Length != 0 && MySpecialDBModel.NewIDValue.Length != 0)
                                                    {
                                                        ClientMySQLGeneralQuery = new MySqlCommand();
                                                        SQLLoopCount = 0;
                                                        while (LoopCount < MySpecialDBModel.IDValue.Length)
                                                        {
                                                            while (SQLLoopCount < TablesNameThatHasPK.Length)
                                                            {
                                                                ClientMySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `" + TablesNameThatHasPK[SQLLoopCount] + "` WHERE `" + TablesPKColumnName[SQLLoopCount] + "`=@" + TablesPKColumnName[SQLLoopCount];
                                                                ClientMySQLGeneralQuery.Parameters.Add("@" + TablesPKColumnName[SQLLoopCount], MySqlDbType.Text).Value = MySpecialDBModel.NewIDValue[LoopCount];
                                                                ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                                                ClientMySQLGeneralQuery.Prepare();
                                                                SQLCount = int.Parse(ClientMySQLGeneralQuery.ExecuteScalar().ToString());
                                                                ClientMySQLGeneralQuery = new MySqlCommand();
                                                                if (SQLCount == 1)
                                                                {
                                                                    IsPKFieldData[LoopCount] = true;
                                                                }
                                                                SQLLoopCount += 1;
                                                            }
                                                            SQLLoopCount = 0;
                                                            LoopCount += 1;
                                                        }
                                                        LoopCount = 0;
                                                        while (LoopCount < IsPKFieldData.Length)
                                                        {
                                                            if (IsPKFieldData[LoopCount] == false)
                                                            {
                                                                CheckIDValueArray = false;
                                                                break;
                                                            }
                                                            LoopCount += 1;
                                                        }
                                                        if (CheckIDValueArray == true) 
                                                        {
                                                            if (MySpecialDBModel.IDValue.Length == MySpecialDBModel.NewIDValue.Length)
                                                            {
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
                                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                                                if (UsedAmountString != null && UsedAmountString.CompareTo("") != 0)
                                                                {
                                                                    DBUsedAmount = ulong.Parse(UsedAmountString);
                                                                    //2 GB
                                                                    if (DBUsedAmount > 2147483648)
                                                                    {
                                                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                                        SodiumSecureMemory.SecureClearString(DBName);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                                        Status = "Error: Please delete some data in your database, you have used more than 2GB";
                                                                        return Status;
                                                                    }
                                                                }
                                                                ClientMySQLGeneralQuery = new MySqlCommand();
                                                                ClientMySQLGeneralQuery.CommandText = QueryString;
                                                                LoopCount = 0;
                                                                while (LoopCount < MySpecialDBModel.IDValue.Length)
                                                                {
                                                                    ClientMySQLGeneralQuery.Parameters.Add("@Old_Update_ID" + LoopCount.ToString(), MySqlDbType.Text).Value = MySpecialDBModel.IDValue[LoopCount];
                                                                    NewDBUsedAmount += (ulong)MySpecialDBModel.IDValue[LoopCount].Length;
                                                                    LoopCount += 1;
                                                                }
                                                                LoopCount = 0;
                                                                while (LoopCount < MySpecialDBModel.IDValue.Length)
                                                                {
                                                                    ClientMySQLGeneralQuery.Parameters.Add("@New_Update_ID" + LoopCount.ToString(), MySqlDbType.Text).Value = MySpecialDBModel.NewIDValue[LoopCount];
                                                                    NewDBUsedAmount += (ulong)MySpecialDBModel.NewIDValue[LoopCount].Length;
                                                                    LoopCount += 1;
                                                                }
                                                                ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                                                ClientMySQLGeneralQuery.Prepare();
                                                                ClientMySQLGeneralQuery.ExecuteNonQuery();
                                                                ClientMySQLDB.ClientMySQLConnection.Close();
                                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                MySQLGeneralQuery = new MySqlCommand();
                                                                MySQLGeneralQuery.CommandText = "UPDATE `Payment` SET `Bytes_Used`=@Bytes_Used WHERE `ID`=@ID";
                                                                MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySpecialDBModel.UniquePaymentID;
                                                                MySQLGeneralQuery.Parameters.Add("@Bytes_Used", MySqlDbType.Text).Value = DBUsedAmount.ToString();
                                                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                                MySQLGeneralQuery.Prepare();
                                                                MySQLGeneralQuery.ExecuteNonQuery();
                                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                                SuccessStatus = "Old foreign keys value have been replaced by new foreign keys value";
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                                return SuccessStatus;
                                                            }
                                                            else if (MySpecialDBModel.IDValue.Length < MySpecialDBModel.NewIDValue.Length)
                                                            {
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
                                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                                                if (UsedAmountString != null && UsedAmountString.CompareTo("") != 0)
                                                                {
                                                                    DBUsedAmount = ulong.Parse(UsedAmountString);
                                                                    //2 GB
                                                                    if (DBUsedAmount > 2147483648)
                                                                    {
                                                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                                        SodiumSecureMemory.SecureClearString(DBName);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                                        Status = "Error: Please delete some data in your database, you have used more than 2GB";
                                                                        return Status;
                                                                    }
                                                                }
                                                                ClientMySQLGeneralQuery = new MySqlCommand();
                                                                ClientMySQLGeneralQuery.CommandText = QueryString;
                                                                LoopCount = 0;
                                                                while (LoopCount < MySpecialDBModel.IDValue.Length)
                                                                {
                                                                    ClientMySQLGeneralQuery.Parameters.Add("@Old_Update_ID" + LoopCount.ToString(), MySqlDbType.Text).Value = MySpecialDBModel.IDValue[LoopCount];
                                                                    NewDBUsedAmount += (ulong)MySpecialDBModel.IDValue[LoopCount].Length;
                                                                    LoopCount += 1;
                                                                }
                                                                LoopCount = 0;
                                                                while (LoopCount < MySpecialDBModel.NewIDValue.Length)
                                                                {
                                                                    ClientMySQLGeneralQuery.Parameters.Add("@New_Update_ID" + LoopCount.ToString(), MySqlDbType.Text).Value = MySpecialDBModel.NewIDValue[LoopCount];
                                                                    NewDBUsedAmount += (ulong)MySpecialDBModel.NewIDValue[LoopCount].Length;
                                                                    LoopCount += 1;
                                                                }
                                                                ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                                                ClientMySQLGeneralQuery.Prepare();
                                                                ClientMySQLGeneralQuery.ExecuteNonQuery();
                                                                ClientMySQLDB.ClientMySQLConnection.Close();
                                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                MySQLGeneralQuery = new MySqlCommand();
                                                                MySQLGeneralQuery.CommandText = "UPDATE `Payment` SET `Bytes_Used`=@Bytes_Used WHERE `ID`=@ID";
                                                                MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MySpecialDBModel.UniquePaymentID;
                                                                MySQLGeneralQuery.Parameters.Add("@Bytes_Used", MySqlDbType.Text).Value = DBUsedAmount.ToString();
                                                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                                MySQLGeneralQuery.Prepare();
                                                                MySQLGeneralQuery.ExecuteNonQuery();
                                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                                SuccessStatus = "Newer Foreign Key Values were replaced by supplying fewer older foreign key values";
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                                return SuccessStatus;
                                                            }
                                                            else
                                                            {
                                                                ClientMySQLDB.ClientMySQLConnection.Close();
                                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserName);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                                Status = "Error: Newer ID value array elements can't be more than older ID value array elements";
                                                                return Status;
                                                            }
                                                        }
                                                        else 
                                                        {
                                                            ClientMySQLDB.ClientMySQLConnection.Close();
                                                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                            SodiumSecureMemory.SecureClearString(DBName);
                                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                            SodiumSecureMemory.SecureClearString(DBUserName);
                                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                            Status = "Error: Please don't input your own foreign key ID values";
                                                            return Status;
                                                        }                                                        
                                                    }
                                                    else
                                                    {
                                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                        SodiumSecureMemory.SecureClearString(DBName);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                        SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                        Status = "Error: IDValue array and NewIDValue array length must not be 0";
                                                        return Status;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ClientMySQLDB.ClientMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                SodiumSecureMemory.SecureClearString(DBName);
                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                SodiumSecureMemory.SecureClearString(DBUserName);
                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                                Status = "Error: You are not allowed to put your own foreign keys values, it must be one of the ID that comes from using API";
                                                return Status;
                                            }
                                        }
                                        else
                                        {
                                            ClientMySQLDB.ClientMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                            SodiumSecureMemory.SecureClearString(DBName);
                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                            SodiumSecureMemory.SecureClearString(DBUserName);
                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                            Status = "Error: You haven't create any tables that has primary key yet";
                                            return Status;
                                        }
                                    }
                                    else
                                    {
                                        Status = "Error: IDValue array must not be 0";
                                        return Status;
                                    }
                                }
                                else
                                {
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                    SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                    SodiumSecureMemory.SecureClearString(DBName);
                                    SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                    SodiumSecureMemory.SecureClearString(DBUserName);
                                    SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                    SodiumSecureMemory.SecureClearString(DBUserPassword);
                                    SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                    Status = "Error: You haven't renew the database";
                                    return Status;
                                }
                            }
                            else
                            {
                                ClientMySQLDB.ClientMySQLConnection.Close();
                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                SodiumSecureMemory.SecureClearString(DBName);
                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                SodiumSecureMemory.SecureClearString(DBUserName);
                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
                                Status = "Error: You have input wrong db credentials";
                                return Status;
                            }
                        }
                        else
                        {
                            ClientMySQLDB.ClientMySQLConnection.Close();
                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBName);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                            SodiumSecureMemory.SecureClearString(DBName);
                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserName);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                            SodiumSecureMemory.SecureClearString(DBUserName);
                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedDBUserPassword);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                            SodiumSecureMemory.SecureClearString(MySpecialDBModel.MyDBCredentialModel.SealedSessionID);
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
                Status = "Error: SpecialDBModel can't be null";
                return Status;
            }
        }
    }
}
