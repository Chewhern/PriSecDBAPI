using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASodium;
using System.Text;
using System.IO;
using MySql.Data.MySqlClient;
using PriSecDBAPI.Model;
using PriSecDBAPI.Helper;


namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstablishSealedBoxDBCredentials : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("byID")]
        public ECDH_ECDSA_Models TempSession(String ClientPathID)
        {
            ECDH_ECDSA_Models MyECDH_ECDSA_Models = new ECDH_ECDSA_Models();
            StringBuilder MyStringBuilder = new StringBuilder();
            Byte[] ServerECDSAPK = new Byte[] { };
            Byte[] ServerECDHSPK = new Byte[] { };
            RevampedKeyPair ServerECDHKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
            RevampedKeyPair ServerECDSAKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
            String Path = "{Path to Sealed Session}";
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                Path += ClientPathID;
                if (Directory.Exists(Path)==true)
                {
                    MyECDH_ECDSA_Models.ECDH_SPK_Base64String = "None";
                    MyECDH_ECDSA_Models.ECDSA_PK_Base64String = "None";
                    MyECDH_ECDSA_Models.ID_Checker_Message = "Error: This sealed session ID already exist";
                }
                else
                {
                    Directory.CreateDirectory(Path);
                    ServerECDSAPK = ServerECDSAKeyPair.PublicKey;
                    ServerECDHSPK = SodiumPublicKeyAuth.Sign(ServerECDHKeyPair.PublicKey, ServerECDSAKeyPair.PrivateKey);
                    MyECDH_ECDSA_Models.ECDH_SPK_Base64String = Convert.ToBase64String(ServerECDHSPK);
                    MyECDH_ECDSA_Models.ECDSA_PK_Base64String = Convert.ToBase64String(ServerECDSAPK);
                    System.IO.File.WriteAllBytes(Path + "/" + "ECDHSK.txt", ServerECDHKeyPair.PrivateKey);
                    MyECDH_ECDSA_Models.ID_Checker_Message = "Success: The sealed session ID had been created";
                }
                System.IO.File.Create(Path + "/Confirmation.txt");
            }
            else
            {
                MyECDH_ECDSA_Models.ECDH_SPK_Base64String = "None";
                MyECDH_ECDSA_Models.ECDSA_PK_Base64String = "None";
                MyECDH_ECDSA_Models.ID_Checker_Message = "Error: Please provide an ID";
            }
            ServerECDHKeyPair.Clear();
            ServerECDSAKeyPair.Clear();
            return MyECDH_ECDSA_Models;
        }

        [HttpGet("DeleteSealedSession")]
        public String DeleteSealedSession(String ClientPathID, String UniquePaymentID, String SignedRandomChallenge)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            CryptographicSecureStrongPasswordGenerator PasswordGenerator = new CryptographicSecureStrongPasswordGenerator();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            Byte[] SignedRandomChallengeByte = new Byte[] { };
            Byte[] RandomChallengeByte = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to Sealed Session}";
            Path += ClientPathID;
            DateTime MyUTC8Time = DateTime.UtcNow.AddHours(8);
            String DatabaseExpirationTimeString = "";
            DateTime DatabaseExpirationTime = new DateTime();
            String DatabaseLoginED25519PK = "";
            Byte[] DatabaseLoginED25519PKByte = new Byte[] { };
            String DatabaseSignedLoginED25519PK = "";
            Byte[] DatabaseSignedLoginED25519PKByte = new Byte[] { };
            Byte[] TestDatabaseLoginED25519PKByte = new Byte[] { };
            int Count = 0;
            int PaymentIDCount = 0;
            DateTime RandomChallengeValidDuration = new DateTime();
            TimeSpan TimeDifference = new TimeSpan();
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    SignedRandomChallengeByte = Convert.FromBase64String(SignedRandomChallenge);
                    myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                    MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Payment` WHERE `ID`=@ID";
                    MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                    MySQLGeneralQuery.Prepare();
                    PaymentIDCount = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                    if (PaymentIDCount == 1) 
                    {
                        MySQLGeneralQuery = new MySqlCommand();
                        MySQLGeneralQuery.CommandText = "SELECT `Expiration_Date` FROM `Payment` WHERE `ID`=@ID";
                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                        MySQLGeneralQuery.Prepare();
                        DatabaseExpirationTimeString = MySQLGeneralQuery.ExecuteScalar().ToString();
                        DatabaseExpirationTime = DateTime.Parse(DatabaseExpirationTimeString);
                        if (DateTime.Compare(MyUTC8Time, DatabaseExpirationTime) <= 0)
                        {
                            MySQLGeneralQuery = new MySqlCommand();
                            MySQLGeneralQuery.CommandText = "SELECT `Signed_ED25519_PK` FROM `Account_Lock` WHERE `Payment_ID`=@Payment_ID";
                            MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            DatabaseSignedLoginED25519PK = MySQLGeneralQuery.ExecuteScalar().ToString();
                            MySQLGeneralQuery = new MySqlCommand();
                            MySQLGeneralQuery.CommandText = "SELECT `ED25519_PK` FROM `Account_Lock` WHERE `Payment_ID`=@Payment_ID";
                            MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                            MySQLGeneralQuery.Prepare();
                            DatabaseLoginED25519PK = MySQLGeneralQuery.ExecuteScalar().ToString();
                            DatabaseSignedLoginED25519PKByte = Convert.FromBase64String(DatabaseSignedLoginED25519PK);
                            DatabaseLoginED25519PKByte = Convert.FromBase64String(DatabaseLoginED25519PK);
                            try
                            {
                                TestDatabaseLoginED25519PKByte = SodiumPublicKeyAuth.Verify(DatabaseSignedLoginED25519PKByte, DatabaseLoginED25519PKByte);
                            }
                            catch
                            {
                                return "Error: Unable to verify the signed ED25519PK through the unsigned ED25519 PK";
                            }
                            if (TestDatabaseLoginED25519PKByte.SequenceEqual(DatabaseLoginED25519PKByte) == false)
                            {
                                return "Error: Database Login ED25519 PK unmatch with Database Signed Login ED25519 PK";
                            }
                            try
                            {
                                RandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, DatabaseLoginED25519PKByte);
                            }
                            catch
                            {
                                return "Error: Unable to verify user signed random challenge through database ED25519 PK";
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
                                RandomChallengeValidDuration = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                TimeDifference = MyUTC8Time.Subtract(RandomChallengeValidDuration);
                                if (TimeDifference.Minutes < 8)
                                {
                                    Directory.Delete(Path,true);
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    MySQLGeneralQuery.ExecuteNonQuery();
                                    MySQLGeneralQuery = new MySqlCommand();
                                    return "Successed: You have succesfully delete the sealed session";
                                }
                                else
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    MySQLGeneralQuery.ExecuteNonQuery();
                                    return "Error: This challenge is no more valid as 8 minutes have passed";
                                }
                            }
                            else
                            {
                                return "Error: This server generated challenge no longer exists or valid";
                            }
                        }
                        else
                        {
                            return "Error: You can't delete the keys used to establish sealed box db credentials as you haven't renew payment";
                        }
                    }
                    else 
                    {
                        return "Error: The payment ID does not exist";
                    }                       
                }
                else
                {
                    return "Error: The corresponding ETLS ID does not exists in the server..";
                }
            }
            else
            {
                return "Error: The ETLS ID mustn't be null";
            }
        }
    }
}
