using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASodium;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;
using PriSecDBAPI.Model;
using PriSecDBAPI.Helper;


namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockDBAccount : ControllerBase
    {
        private GenerateUserForDB GenerateDBUserMySQLCon = new GenerateUserForDB();
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpPost]
        public String LockDBAccountFunction(LockDBAccountModel MyLockModel)
        {
            String Status = "";
            String Path = "{Path to Sealed Session}";
            Byte[] CipheredDBUserNameByte = new Byte[] { };
            Byte[] DBUserNameByte = new Byte[] { };
            String DBUserName = "";
            Byte[] ServerSealedDHPrivateKeyByte = new Byte[] { };
            Byte[] ServerSealedDHPublicKeyByte = new Byte[] { };
            int PaymentIDCount = 0;
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DatabaseExpirationTime = new DateTime();
            DateTime RandomChallengeValidDateTime = new DateTime();
            TimeSpan TimeDifference = new TimeSpan();
            Byte[] SignedED25519PKByte = new Byte[] { };
            Byte[] ED25519PKByte = new Byte[] { };
            Byte[] VerifiedED25519PKByte = new Byte[] { };
            Byte[] SignedRandomChallengeByte = new Byte[] { };
            Byte[] RandomChallengeByte = new Byte[] { };
            int Count = 0;
            if (MyLockModel != null)
            {
                if (MyLockModel.SealedSessionID != null)
                {
                    Path += MyLockModel.SealedSessionID;
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
                            CipheredDBUserNameByte = Convert.FromBase64String(MyLockModel.SealedDBUserName);
                        }
                        catch
                        {
                            Status = "Error: Encrypted DB User Name is not in base 64 format";
                            return Status;
                        }
                        ServerSealedDHPublicKeyByte = SodiumScalarMult.Base(ServerSealedDHPrivateKeyByte);
                        try
                        {
                            DBUserNameByte = SodiumSealedPublicKeyBox.Open(CipheredDBUserNameByte, ServerSealedDHPublicKeyByte, ServerSealedDHPrivateKeyByte,true);
                        }
                        catch
                        {
                            Status = "Error: Unable to decrypt sealed DB credentials";
                            return Status;
                        }
                        DBUserName = Encoding.UTF8.GetString(DBUserNameByte);
                        SodiumSecureMemory.SecureClearBytes(ServerSealedDHPublicKeyByte);
                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Payment` WHERE `ID`=@ID";
                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyLockModel.UniquePaymentID;
                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                        MySQLGeneralQuery.Prepare();
                        PaymentIDCount = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                        if (PaymentIDCount == 1)
                        {
                            MySQLGeneralQuery = new MySqlCommand();
                            MySQLGeneralQuery.CommandText = "SELECT `Expiration_Date` FROM `Payment` WHERE `ID`=@ID";
                            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = MyLockModel.UniquePaymentID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            DatabaseExpirationTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                            if (DateTime.Compare(MyUTC8DateTime, DatabaseExpirationTime) <= 0)
                            {
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "SELECT `Signed_ED25519_PK` FROM `Account_Lock` WHERE `Payment_ID`=@Payment_ID";
                                MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = MyLockModel.UniquePaymentID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                SignedED25519PKByte = Convert.FromBase64String(MySQLGeneralQuery.ExecuteScalar().ToString());
                                MySQLGeneralQuery = new MySqlCommand();
                                MySQLGeneralQuery.CommandText = "SELECT `ED25519_PK` FROM `Account_Lock` WHERE `Payment_ID`=@Payment_ID";
                                MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = MyLockModel.UniquePaymentID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                ED25519PKByte = Convert.FromBase64String(MySQLGeneralQuery.ExecuteScalar().ToString());
                                try
                                {
                                    VerifiedED25519PKByte = SodiumPublicKeyAuth.Verify(SignedED25519PKByte, ED25519PKByte);
                                }
                                catch
                                {
                                    Status = "Error: Can't verify the signed ED25519PK with the unsigned ED25519PK";
                                    return Status;
                                }
                                if (VerifiedED25519PKByte.SequenceEqual(ED25519PKByte) == true)
                                {
                                    try
                                    {
                                        SignedRandomChallengeByte = Convert.FromBase64String(MyLockModel.SignedRandomChallenge);
                                    }
                                    catch
                                    {
                                        Status = "Error: Signed random challenge can't be convert from Base64 to byte array";
                                        return Status;
                                    }
                                    try
                                    {
                                        RandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, ED25519PKByte);
                                    }
                                    catch
                                    {
                                        Status = "Error: Random challenge can't be verified through ED25519PK that's in database";
                                        return Status;
                                    }
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                    if (Count == 1)
                                    {
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "SELECT `Valid_Duration` FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        RandomChallengeValidDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        TimeDifference = MyUTC8DateTime.Subtract(RandomChallengeValidDateTime);
                                        if (TimeDifference.TotalMinutes < 8)
                                        {
                                            if (DBUserName.CompareTo("Chew") == 0 || DBUserName.CompareTo("Lynas") == 0 || DBUserName.CompareTo("Hern") == 0 || DBUserName.CompareTo("Chrono") == 0)
                                            {
                                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                                SodiumSecureMemory.SecureClearString(DBUserName);
                                                SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
                                                Status = "Error: You specified the db user account that belongs to the server rather than client's db user account";
                                                return Status;
                                            }
                                            GenerateDBUserMySQLCon.LoadConnection(ref ExceptionString);
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "ALTER USER " + "'" + DBUserName + "'" + "@'localhost' ACCOUNT LOCK";
                                            MySQLGeneralQuery.Connection = GenerateDBUserMySQLCon.GenerateDBUserConnection;
                                            MySQLGeneralQuery.Prepare();
                                            MySQLGeneralQuery.ExecuteNonQuery();
                                            GenerateDBUserMySQLCon.GenerateDBUserConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                            SodiumSecureMemory.SecureClearString(DBUserName);
                                            SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
                                            Status = "Success: The specified DB user account have been locked";
                                            return Status;
                                        }
                                        else
                                        {
                                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                            SodiumSecureMemory.SecureClearString(DBUserName);
                                            SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
                                            Status = "Error: The random challenge is no longer valid";
                                            return Status;
                                        }
                                    }
                                    else
                                    {
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
                                        Status = "Error: Random challenge does not exist in database";
                                        return Status;
                                    }
                                }
                                else
                                {
                                    SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                    SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                    SodiumSecureMemory.SecureClearString(DBUserName);
                                    SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
                                    Status = "Error: The unsigned version of ED25519PK is unmatch with the verified version of ED25519PK";
                                    return Status;
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                SodiumSecureMemory.SecureClearString(DBUserName);
                                SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
                                Status = "Error: You haven't renew the database hosting service";
                                return Status;
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                            SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                            SodiumSecureMemory.SecureClearString(DBUserName);
                            SodiumSecureMemory.SecureClearString(MyLockModel.SealedSessionID);
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
                Status = "Error: LockDBAccountModel can't be null";
                return Status;
            }
        }
    }
}
