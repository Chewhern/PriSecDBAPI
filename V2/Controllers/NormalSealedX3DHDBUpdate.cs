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
    public class NormalSealedX3DHDBUpdate : ControllerBase
    {

        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpPost]
        public String UpdateDBConfidentialData(NormalDBUpdateModel MyDBUpdateModel)
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
            int SQLLoopCount = 1;
            int LoopCount = 0;
            String UsedAmountString = null;
            ulong DBUsedAmount = 0;
            String SuccessStatus = "";
            if (MyDBUpdateModel != null)
            {
                if (MyDBUpdateModel.MyDBCredentialModel.SealedSessionID != null)
                {
                    Path += MyDBUpdateModel.MyDBCredentialModel.SealedSessionID;
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
                            CipheredDBNameByte = Convert.FromBase64String(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB Name is not in base 64 format";
                            return Status;
                        }
                        try
                        {
                            CipheredDBUserNameByte = Convert.FromBase64String(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB User Name is not in base 64 format";
                            return Status;
                        }
                        try
                        {
                            CipheredDBUserPasswordByte = Convert.FromBase64String(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
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
                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyDBUpdateModel.UniquePaymentID;
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
                                MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyDBUpdateModel.UniquePaymentID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                DatabaseExpirationTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (DateTime.Compare(MyUTC8DateTime, DatabaseExpirationTime) <= 0)
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT * FROM `X3SDH` WHERE `Payment_ID`=@Payment_ID";
                                    MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = MyDBUpdateModel.UniquePaymentID;
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
                                        QueryStringByte = Convert.FromBase64String(MyDBUpdateModel.Base64QueryString);
                                    }
                                    catch
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: The query must be in base64 format";
                                        return Status;
                                    }
                                    QueryString = Encoding.UTF8.GetString(QueryStringByte);
                                    if (QueryString.ToLower().Contains("update") == false || (QueryString.ToLower().Contains("insert") == true || QueryString.ToLower().Contains("delete") == true || QueryString.ToLower().Contains("select") == true || QueryString.ToLower().Contains(";") == true || QueryString.ToLower().Contains("'") == true))
                                    {
                                        Status = "Error: You didn't pass in an update query string";
                                        return Status;
                                    }
                                    ClientMySQLGeneralQuery = new MySqlCommand();
                                    ClientMySQLGeneralQuery.CommandText = QueryString;
                                    if (MyDBUpdateModel.Base64ParameterName.Length != 0 && MyDBUpdateModel.Base64ParameterValue.Length != 0)
                                    {
                                        if (MyDBUpdateModel.Base64ParameterName.Length == MyDBUpdateModel.Base64ParameterValue.Length)
                                        {
                                            while (LoopCount < MyDBUpdateModel.Base64ParameterName.Length)
                                            {
                                                RevampedKeyPair AliceIdentityKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                                                RevampedKeyPair AliceEphemeralKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
                                                ParameterNameByte = Convert.FromBase64String(MyDBUpdateModel.Base64ParameterName[LoopCount]);
                                                ParameterName = Encoding.UTF8.GetString(ParameterNameByte);
                                                ParameterValueByte = Convert.FromBase64String(MyDBUpdateModel.Base64ParameterValue[LoopCount]);
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
                                                if (MyDBUpdateModel.IsXSalsa20Poly1305 == true)
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
                                                ClientMySQLGeneralQuery.Parameters.Add("@" + ParameterName, MySqlDbType.Text).Value = Convert.ToBase64String(EncryptedParameterValueByte);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret1);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret2);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret3);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret4);
                                                AliceIdentityKeyPair.Clear();
                                                AliceEphemeralKeyPair.Clear();
                                                LoopCount += 1;
                                            }
                                            ClientMySQLGeneralQuery.Parameters.Add("@Update_ID", MySqlDbType.Text).Value = MyDBUpdateModel.IDUsedToUpdate;
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
                                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                                    SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                                    SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                                    SodiumSecureMemory.SecureClearString(DBName);
                                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                    SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                    SodiumSecureMemory.SecureClearString(DBUserName);
                                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                                    SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                                    SodiumSecureMemory.SecureClearString(DBUserPassword);
                                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                                    Status = "Error: Please delete some data in your database, you have used more than 2GB";
                                                    return Status;
                                                }
                                            }
                                            ClientMySQLGeneralQuery.Connection = ClientMySQLDB.ClientMySQLConnection;
                                            ClientMySQLGeneralQuery.Prepare();
                                            ClientMySQLGeneralQuery.ExecuteNonQuery();
                                            ClientMySQLDB.ClientMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                            SQLLoopCount = 1;
                                            ClientMySQLDB.LoadConnection(DBName, DBUserName, DBUserPassword, ref ClientExceptionString);
                                            ClientSQLQuery = new MySqlCommand();
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
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "UPDATE `Payment` SET `Bytes_Used`=@Bytes_Used WHERE `ID`=@ID";
                                            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyDBUpdateModel.UniquePaymentID;
                                            MySQLGeneralQuery.Parameters.Add("@Bytes_Used", MySqlDbType.Text).Value = UsedAmountString;
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            MySQLGeneralQuery.ExecuteNonQuery();
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SuccessStatus = "Row's columns values with ID " + MyDBUpdateModel.IDUsedToUpdate + " has been updated";
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                            SodiumSecureMemory.SecureClearString(DBName);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                            SodiumSecureMemory.SecureClearString(DBUserName);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                            return SuccessStatus;
                                        }
                                        else
                                        {
                                            ClientMySQLDB.ClientMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                            SodiumSecureMemory.SecureClearString(DBName);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                            SodiumSecureMemory.SecureClearString(DBUserName);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                            Status = "Error: Parameter name array length and parameter value array length is not the same";
                                            return Status;
                                        }
                                    }
                                    else
                                    {
                                        ClientMySQLDB.ClientMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                        Status = "Error: Parameter name array and parameter value array must not be null or empty";
                                        return Status;
                                    }
                                }
                                else
                                {
                                    ClientMySQLDB.ClientMySQLConnection.Close();
                                    SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                    SodiumSecureMemory.SecureClearString(DBName);
                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                    SodiumSecureMemory.SecureClearString(DBUserName);
                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                    SodiumSecureMemory.SecureClearString(DBUserPassword);
                                    SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                    Status = "Error: You haven't renew the database";
                                    return Status;
                                }
                            }
                            else
                            {
                                ClientMySQLDB.ClientMySQLConnection.Close();
                                SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                                SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                SodiumSecureMemory.SecureClearString(DBName);
                                SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                SodiumSecureMemory.SecureClearString(DBUserName);
                                SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                SodiumSecureMemory.SecureClearString(DBUserPassword);
                                SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
                                Status = "Error: You have input wrong db credentials";
                                return Status;
                            }
                        }
                        else
                        {
                            ClientMySQLDB.ClientMySQLConnection.Close();
                            SodiumSecureMemory.SecureClearString(ClientMySQLDB.ClientMySQLConnection.ConnectionString);
                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBName);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBNameByte);
                            SodiumSecureMemory.SecureClearString(DBName);
                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserName);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                            SodiumSecureMemory.SecureClearString(DBUserName);
                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedDBUserPassword);
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                            SodiumSecureMemory.SecureClearString(DBUserPassword);
                            SodiumSecureMemory.SecureClearString(MyDBUpdateModel.MyDBCredentialModel.SealedSessionID);
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
                Status = "Error: NormalDBUpdateModel can't be null";
                return Status;
            }
        }

    }
}
