using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASodium;
using MySql.Data.MySqlClient;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using PriSecDBAPI.Model;
using PriSecDBAPI.Helper;

namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private CryptographicSecureIDGenerator myCryptographicSecureIDGenerator = new CryptographicSecureIDGenerator();
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet]
        public LoginModels RequestChallenge() 
        {
            LoginModels MyLoginModels = new LoginModels();
            PublicKeyAuthSealBox MySealBox = new PublicKeyAuthSealBox();
            Byte[] RandomData = new Byte[128];
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            int Count = 0;
            String RequestID = myCryptographicSecureIDGenerator.GenerateMinimumAmountOfUniqueString(24);
            String ExceptionString = "";
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(RandomData);
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
            MySQLGeneralQuery = new MySqlCommand();
            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
            MySQLGeneralQuery.Prepare();
            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
            while (Count != 0) 
            {
                RandomData = new Byte[128];
                rngCsp.GetBytes(RandomData);
                MySQLGeneralQuery = new MySqlCommand();
                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                MySQLGeneralQuery.Prepare();
                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
            }
            MySQLGeneralQuery = new MySqlCommand();
            MySQLGeneralQuery.CommandText = "INSERT INTO `Random_Challenge`(`Challenge`, `Valid_Duration`, `ID`) VALUES (@Challenge,@Valid_Duration,@ID)";
            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
            MySQLGeneralQuery.Parameters.Add("@Valid_Duration", MySqlDbType.DateTime).Value = MyUTC8DateTime;
            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = RequestID;
            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
            MySQLGeneralQuery.Prepare();
            MySQLGeneralQuery.ExecuteNonQuery();
            MySealBox = SodiumPublicKeyAuth.SealedSign(RandomData);
            MyLoginModels.RequestStatus = "Success";
            MyLoginModels.ServerECDSAPKBase64String = Convert.ToBase64String(MySealBox.PublicKey);
            MyLoginModels.SignedRandomChallengeBase64String = Convert.ToBase64String(MySealBox.SignatureMessage);
            return MyLoginModels;
        }
    }
}
