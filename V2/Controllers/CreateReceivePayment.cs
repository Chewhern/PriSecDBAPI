using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ASodium;
using System.Text;
using MySql.Data.MySqlClient;
using PayPalCheckoutSdk.Orders;
using PriSecDBAPI.Model;
using PriSecDBAPI.Helper;


namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateReceivePayment : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();
        private GenerateMySQLDB MyGenerateMySQLDBConnection = new GenerateMySQLDB();
        private GenerateUserForDB MyGenerateUserForDBConnection = new GenerateUserForDB();
        private GrantPermissionDB MyGrantPermissionDBConnection = new GrantPermissionDB();
        private CryptographicSecureIDGenerator MyIDGenerator = new CryptographicSecureIDGenerator();

        [HttpGet]
        public CheckOutPageHolderModel CreateCheckOutPage() 
        {
            Double OriginalPrice = 0.00;
            Double HandlingPrice = 1.00;
            Double TotalPrice = 0.00;
            Double ItemPrice = 0.00;
            String ItemDescription = "";
            String MyInvoiceID = MyIDGenerator.GenerateUniqueString();
            String OrderID = "";
            String RedirectLink = "";
            OriginalPrice = 4.00;
            ItemPrice = OriginalPrice;
            TotalPrice = OriginalPrice + HandlingPrice;
            ItemDescription = "2 GB Database";
            var CreateOrderResponse = PayPalCreateOrder.CreateOrder(MyInvoiceID, TotalPrice, OriginalPrice, HandlingPrice, ItemPrice, ItemDescription).Result;
            var createOrderResult = CreateOrderResponse.Result<Order>();
            OrderID = createOrderResult.Id;
            foreach (LinkDescription link in createOrderResult.Links)
            {
                if (link.Rel.CompareTo("approve") == 0)
                {
                    RedirectLink = link.Href;
                }
            }
            CheckOutPageHolderModel PageHolder = new CheckOutPageHolderModel();
            PageHolder.PayPalOrderID = OrderID;
            PageHolder.CheckOutPageUrl = RedirectLink;
            PageHolder.InvoiceID = MyInvoiceID;
            return PageHolder;
        }

        [HttpGet("CheckPayment")]
        public PaymentModel CheckPaymentAndCreateFolder(String ClientPathID, String CipheredSignedOrderID, String CipheredSignedLoginED25519PK, String EncryptedSignedSignedLoginED25519PK, String CipheredSignedSignedSealedDHX25519PK, String CipheredSignedSealedDHED25519PK, String CipheredSignedSignedSealedX3DHSPKX25519PK, String CipheredSignedSealedX3DHSPKED25519PK, String CipheredSignedSignedSealedX3DHIKX25519PK, String CipheredSignedSealedX3DHIKED25519PK, String CipheredSignedSignedSealedX3DHOPKX25519PK, String CipheredSignedSealedX3DHOPKED25519PK)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            CryptographicSecureStrongPasswordGenerator PasswordGenerator = new CryptographicSecureStrongPasswordGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedLoginED25519PK = "";
            Boolean DecodingCipheredSignedLoginED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedLoginED25519PKChecker = true;
            Byte[] CipheredSignedLoginED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredLoginED25519PKByteChecker = true;
            Byte[] CipheredLoginED25519PKByte = new Byte[] { };
            Byte[] LoginED25519PKByte = new Byte[] { };
            String DecodedEncryptedSignedSignedLoginED25519PK = "";
            Boolean DecodingEncryptedSignedSignedLoginED25519PKChecker = true;
            Boolean ConvertFromBase64EncryptedSignedSignedLoginED25519PKChecker = true;
            Byte[] EncryptedSignedSignedLoginED25519PKByte = new Byte[] { };
            Boolean VerifyEncryptedSignedLoginED25519PKByteChecker = true;
            Byte[] EncryptedSignedLoginED25519PKByte = new Byte[] { };
            Byte[] SignedLoginED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedDHX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedDHX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedDHX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedDHX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedDHX25519PKChecker = true;
            Byte[] CipheredSignedSealedDHX25519PKByte = new Byte[] { };
            Byte[] SignedSealedDHX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedDHED25519PK = "";
            Boolean DecodingCipheredSignedSealedDHED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedDHED25519PKChecker = true;
            Byte[] CipheredSignedSealedDHED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedDHED25519PKChecker = true;
            Byte[] CipheredSealedDHED25519PKByte = new Byte[] { };
            Byte[] SealedDHED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedX3DHSPKX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedX3DHSPKX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedX3DHSPKX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedX3DHSPKX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedX3DHSPKX25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHSPKX25519PKByte = new Byte[] { };
            Byte[] SignedSealedX3DHSPKX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedX3DHSPKED25519PK = "";
            Boolean DecodingCipheredSignedSealedX3DHSPKED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedX3DHSPKED25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHSPKED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedX3DHSPKED25519PKChecker = true;
            Byte[] CipheredSealedX3DHSPKED25519PKByte = new Byte[] { };
            Byte[] SealedX3DHSPKED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedX3DHIKX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedX3DHIKX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedX3DHIKX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedX3DHIKX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedX3DHIKX25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHIKX25519PKByte = new Byte[] { };
            Byte[] SignedSealedX3DHIKX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedX3DHIKED25519PK = "";
            Boolean DecodingCipheredSignedSealedX3DHIKED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedX3DHIKED25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHIKED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedX3DHIKED25519PKChecker = true;
            Byte[] CipheredSealedX3DHIKED25519PKByte = new Byte[] { };
            Byte[] SealedX3DHIKED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedX3DHOPKX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedX3DHOPKX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedX3DHOPKX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedX3DHOPKX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedX3DHOPKX25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHOPKX25519PKByte = new Byte[] { };
            Byte[] SignedSealedX3DHOPKX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedX3DHOPKED25519PK = "";
            Boolean DecodingCipheredSignedSealedX3DHOPKED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedX3DHOPKED25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHOPKED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedX3DHOPKED25519PKChecker = true;
            Byte[] CipheredSealedX3DHOPKED25519PKByte = new Byte[] { };
            Byte[] SealedX3DHOPKED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedOrderID = "";
            Boolean DecodingCipheredSignedOrderIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedOrderIDChecker = true;
            Byte[] CipheredSignedOrderIDByte = new Byte[] { };
            Boolean VerifyCipheredOrderIDByteChecker = true;
            Byte[] CipheredOrderIDByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            String OrderID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            Byte[] ClientECDHPKByte = new Byte[] { };
            Byte[] TestLoginED25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedDHX25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedX3DHSPKX25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedX3DHIKX25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedX3DHOPKX25519PKByte = new Byte[] { };
            Boolean SealedDHX25519PKBooleanChecker = true;
            Boolean SealedX3DHSPKX25519PKBooleanChecker = true;
            Boolean SealedX3DHIKX25519PKBooleanChecker = true;
            Boolean SealedX3DHOPKX25519PKBooleanChecker = true;
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS Session}";
            Path += ClientPathID;
            String UniquePaymentID = IDGenerator.GenerateUniqueString();
            DateTime MyUTC8Time = DateTime.UtcNow.AddHours(8);
            DateTime DatabaseExpirationTime = MyUTC8Time.AddDays(30);
            PaymentModel MyPaymentModel = new PaymentModel();
            String DBName = IDGenerator.GenerateUniqueString();
            if (DBName.Length > 16) 
            {
                DBName = DBName.Substring(0, 16);
            }
            Byte[] DBNameByte = Encoding.UTF8.GetBytes(DBName);
            Byte[] CipheredDBNameByte = new Byte[] { };
            String DBUserName = IDGenerator.GenerateUniqueString();
            if (DBUserName.Length > 16) 
            {
                DBUserName = DBUserName.Substring(0, 16);
            }
            Byte[] DBUserNameByte = Encoding.UTF8.GetBytes(DBUserName);
            Byte[] CipheredDBUserNameByte = new Byte[] { };
            String OriginalDBUserPassword = PasswordGenerator.GenerateUniqueString();
            String DBUserPassword = "";
            if (OriginalDBUserPassword.Length > 80) 
            {
                DBUserPassword = OriginalDBUserPassword.Substring(0, 80);
            }
            else 
            {
                DBUserPassword = OriginalDBUserPassword;
            }
            Byte[] DBUserPasswordByte = Encoding.UTF8.GetBytes(DBUserPassword);
            Byte[] CipheredDBUserPasswordByte = new Byte[] { };
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    ClientECDHPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDHPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedOrderIDChecker, ref DecodedCipheredSignedOrderID, CipheredSignedOrderID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedLoginED25519PKChecker, ref DecodedCipheredSignedLoginED25519PK, CipheredSignedLoginED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingEncryptedSignedSignedLoginED25519PKChecker, ref DecodedEncryptedSignedSignedLoginED25519PK, EncryptedSignedSignedLoginED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedDHED25519PKChecker, ref DecodedCipheredSignedSealedDHED25519PK, CipheredSignedSealedDHED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedDHX25519PKChecker, ref DecodedCipheredSignedSignedSealedDHX25519PK, CipheredSignedSignedSealedDHX25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedX3DHSPKED25519PKChecker, ref DecodedCipheredSignedSealedX3DHSPKED25519PK, CipheredSignedSealedX3DHSPKED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedX3DHSPKX25519PKChecker, ref DecodedCipheredSignedSignedSealedX3DHSPKX25519PK, CipheredSignedSignedSealedX3DHSPKX25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedX3DHIKED25519PKChecker, ref DecodedCipheredSignedSealedX3DHIKED25519PK, CipheredSignedSealedX3DHIKED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedX3DHIKX25519PKChecker, ref DecodedCipheredSignedSignedSealedX3DHIKX25519PK, CipheredSignedSignedSealedX3DHIKX25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedX3DHOPKED25519PKChecker, ref DecodedCipheredSignedSealedX3DHOPKED25519PK, CipheredSignedSealedX3DHOPKED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedX3DHOPKX25519PKChecker, ref DecodedCipheredSignedSignedSealedX3DHOPKX25519PK, CipheredSignedSignedSealedX3DHOPKX25519PK);
                    if(DecodingCipheredSignedOrderIDChecker==true && DecodingCipheredSignedLoginED25519PKChecker==true && DecodingEncryptedSignedSignedLoginED25519PKChecker==true && DecodingCipheredSignedSealedDHED25519PKChecker==true && DecodingCipheredSignedSignedSealedDHX25519PKChecker==true && DecodingCipheredSignedSealedX3DHSPKED25519PKChecker==true && DecodingCipheredSignedSignedSealedX3DHSPKX25519PKChecker==true && DecodingCipheredSignedSealedX3DHIKED25519PKChecker==true && DecodingCipheredSignedSignedSealedX3DHIKX25519PKChecker==true && DecodingCipheredSignedSealedX3DHOPKED25519PKChecker==true && DecodingCipheredSignedSignedSealedX3DHOPKX25519PKChecker == true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedOrderIDChecker, ref CipheredSignedOrderIDByte, DecodedCipheredSignedOrderID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedLoginED25519PKChecker, ref CipheredSignedLoginED25519PKByte, DecodedCipheredSignedLoginED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64EncryptedSignedSignedLoginED25519PKChecker, ref EncryptedSignedSignedLoginED25519PKByte, DecodedEncryptedSignedSignedLoginED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedDHED25519PKChecker, ref CipheredSignedSealedDHED25519PKByte, DecodedCipheredSignedSealedDHED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedDHX25519PKChecker, ref CipheredSignedSignedSealedDHX25519PKByte, DecodedCipheredSignedSignedSealedDHX25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedX3DHSPKED25519PKChecker, ref CipheredSignedSealedX3DHSPKED25519PKByte, DecodedCipheredSignedSealedX3DHSPKED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedX3DHSPKX25519PKChecker, ref CipheredSignedSignedSealedX3DHSPKX25519PKByte, DecodedCipheredSignedSignedSealedX3DHSPKX25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedX3DHIKED25519PKChecker, ref CipheredSignedSealedX3DHIKED25519PKByte, DecodedCipheredSignedSealedX3DHIKED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedX3DHIKX25519PKChecker, ref CipheredSignedSignedSealedX3DHIKX25519PKByte, DecodedCipheredSignedSignedSealedX3DHIKX25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedX3DHOPKED25519PKChecker, ref CipheredSignedSealedX3DHOPKED25519PKByte, DecodedCipheredSignedSealedX3DHOPKED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedX3DHOPKX25519PKChecker, ref CipheredSignedSignedSealedX3DHOPKX25519PKByte, DecodedCipheredSignedSignedSealedX3DHOPKX25519PK);
                        if (ConvertFromBase64CipheredSignedOrderIDChecker==true && ConvertFromBase64CipheredSignedLoginED25519PKChecker==true && ConvertFromBase64EncryptedSignedSignedLoginED25519PKChecker==true && ConvertFromBase64CipheredSignedSealedDHED25519PKChecker==true && ConvertFromBase64CipheredSignedSignedSealedDHX25519PKChecker==true && ConvertFromBase64CipheredSignedSealedX3DHSPKED25519PKChecker==true && ConvertFromBase64CipheredSignedSignedSealedX3DHSPKX25519PKChecker==true && ConvertFromBase64CipheredSignedSealedX3DHIKED25519PKChecker==true && ConvertFromBase64CipheredSignedSignedSealedX3DHIKX25519PKChecker==true && ConvertFromBase64CipheredSignedSealedX3DHOPKED25519PKChecker==true && ConvertFromBase64CipheredSignedSignedSealedX3DHOPKX25519PKChecker==true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredOrderIDByteChecker, ref CipheredOrderIDByte, CipheredSignedOrderIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredLoginED25519PKByteChecker, ref CipheredLoginED25519PKByte, CipheredSignedLoginED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyEncryptedSignedLoginED25519PKByteChecker, ref EncryptedSignedLoginED25519PKByte, EncryptedSignedSignedLoginED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedDHED25519PKChecker, ref CipheredSealedDHED25519PKByte, CipheredSignedSealedDHED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedDHX25519PKChecker, ref CipheredSignedSealedDHX25519PKByte, CipheredSignedSignedSealedDHX25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedX3DHSPKED25519PKChecker, ref CipheredSealedX3DHSPKED25519PKByte, CipheredSignedSealedX3DHSPKED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedX3DHSPKX25519PKChecker, ref CipheredSignedSealedX3DHSPKX25519PKByte, CipheredSignedSignedSealedX3DHSPKX25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedX3DHIKED25519PKChecker, ref CipheredSealedX3DHIKED25519PKByte, CipheredSignedSealedX3DHIKED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedX3DHIKX25519PKChecker, ref CipheredSignedSealedX3DHIKX25519PKByte, CipheredSignedSignedSealedX3DHIKX25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedX3DHOPKED25519PKChecker, ref CipheredSealedX3DHOPKED25519PKByte, CipheredSignedSealedX3DHOPKED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedX3DHOPKX25519PKChecker, ref CipheredSignedSealedX3DHOPKX25519PKByte, CipheredSignedSignedSealedX3DHOPKX25519PKByte, ClientECDSAPKByte);
                            if (VerifyCipheredOrderIDByteChecker==true && VerifyCipheredLoginED25519PKByteChecker==true ) 
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredOrderIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredOrderIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredOrderIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    MyPaymentModel.Status = "Error: Unable to decrypt ETLS signed order ID..";
                                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                    return MyPaymentModel;
                                }
                                OrderID = Encoding.UTF8.GetString(PlainText);
                                try
                                {
                                    var captureOrderResponse = PaypalCaptureOrder.CaptureOrder(OrderID).Result;
                                    var captureOrderResult = captureOrderResponse.Result<Order>();
                                    var captureId = "";
                                    foreach (PurchaseUnit purchaseUnit in captureOrderResult.PurchaseUnits)
                                    {
                                        foreach (Capture capture in purchaseUnit.Payments.Captures)
                                        {
                                            captureId = capture.Id;
                                        }
                                    }
                                    if (captureId.CompareTo("") == 0)
                                    {
                                        MyPaymentModel.Status = "Error: You haven't made payment/You have made the payment again on the same order ID";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                }
                                catch
                                {
                                    MyPaymentModel.Status = "Error: You haven't made payment/You have made the payment again on the same order ID";
                                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                    return MyPaymentModel;
                                }
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredLoginED25519PKByte.Length - NonceByte.Length];
                                Array.Copy(CipheredLoginED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredLoginED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    MyPaymentModel.Status = "Error: Unable to decrypt ETLS signed user sent login ed25519 pk..";
                                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                    return MyPaymentModel;
                                }
                                LoginED25519PKByte = PlainText;
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[EncryptedSignedLoginED25519PKByte.Length - NonceByte.Length];
                                Array.Copy(EncryptedSignedLoginED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(EncryptedSignedLoginED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    MyPaymentModel.Status = "Error: Unable to decrypt ETLS signed user sent user signed login ed25519 pk..";
                                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                    return MyPaymentModel;
                                }
                                SignedLoginED25519PKByte = PlainText;
                                try 
                                {
                                    TestLoginED25519PKByte = SodiumPublicKeyAuth.Verify(SignedLoginED25519PKByte, LoginED25519PKByte);
                                }
                                catch 
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    MyPaymentModel.Status = "Error: Unable to verify user signed login ED25519PK with corresponding sent user login ED25519PK";
                                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                    return MyPaymentModel;
                                }
                                if (LoginED25519PKByte.SequenceEqual(TestLoginED25519PKByte) == true) 
                                {
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedDHED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedDHED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedDHED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS signed user sent user sealed diffie hellman digital signature public key";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SealedDHED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedDHX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedDHX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedDHX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS signed user sent user signed sealed diffie hellman key exchange public key";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SignedSealedDHX25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedX3DHSPKED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedX3DHSPKED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedX3DHSPKED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS user sent user sealed diffie hellman digital signature public key for X3DH SPK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SealedX3DHSPKED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedX3DHSPKX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedX3DHSPKX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedX3DHSPKX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS user sent user signed sealed diffie hellman key exchange public key for X3DH SPK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SignedSealedX3DHSPKX25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedX3DHIKED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedX3DHIKED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedX3DHIKED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS user sent user sealed diffie hellman digital signature public key for X3DH IK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SealedX3DHIKED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedX3DHIKX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedX3DHIKX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedX3DHIKX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS user sent user signed sealed diffie hellman key exchange public key for X3DH IK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SignedSealedX3DHIKX25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedX3DHOPKED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedX3DHOPKED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedX3DHOPKED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS user sent user sealed diffie hellman digital signature public key for X3DH OPK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SealedX3DHOPKED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedX3DHOPKX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedX3DHOPKX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedX3DHOPKX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        MyPaymentModel.Status = "Error: Unable to decrypt ETLS user sent user signed sealed diffie hellman key exchange public key for X3DH OPK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SignedSealedX3DHOPKX25519PKByte = PlainText;
                                    try 
                                    {
                                        VerifiedSealedDHX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedDHX25519PKByte, SealedDHED25519PKByte);
                                    }
                                    catch 
                                    {
                                        MyPaymentModel.Status = "Error: Unable to verify user signed sealed DH X25519PK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    try
                                    {
                                        VerifiedSealedX3DHSPKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedX3DHSPKX25519PKByte, SealedX3DHSPKED25519PKByte);
                                    }
                                    catch
                                    {
                                        MyPaymentModel.Status = "Error: Unable to verify user signed sealed X3DH SPK X25519PK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    try
                                    {
                                        VerifiedSealedX3DHIKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedX3DHIKX25519PKByte, SealedX3DHIKED25519PKByte);
                                    }
                                    catch
                                    {
                                        MyPaymentModel.Status = "Error: Unable to verify user signed sealed X3DH IK X25519PK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    try
                                    {
                                        VerifiedSealedX3DHOPKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedX3DHOPKX25519PKByte, SealedX3DHOPKED25519PKByte);
                                    }
                                    catch
                                    {
                                        MyPaymentModel.Status = "Error: Unable to verify user signed sealed X3DH OPK X25519PK";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                    SealedDHX25519PKBooleanChecker = (VerifiedSealedDHX25519PKByte.SequenceEqual(VerifiedSealedX3DHSPKX25519PKByte)|| VerifiedSealedDHX25519PKByte.SequenceEqual(VerifiedSealedX3DHIKX25519PKByte) || VerifiedSealedDHX25519PKByte.SequenceEqual(VerifiedSealedX3DHOPKX25519PKByte));
                                    SealedX3DHSPKX25519PKBooleanChecker = (VerifiedSealedX3DHSPKX25519PKByte.SequenceEqual(VerifiedSealedDHX25519PKByte) || VerifiedSealedX3DHSPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHIKX25519PKByte) || VerifiedSealedX3DHSPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHOPKX25519PKByte));
                                    SealedX3DHIKX25519PKBooleanChecker = (VerifiedSealedX3DHIKX25519PKByte.SequenceEqual(VerifiedSealedDHX25519PKByte) || VerifiedSealedX3DHIKX25519PKByte.SequenceEqual(VerifiedSealedX3DHSPKX25519PKByte) || VerifiedSealedX3DHIKX25519PKByte.SequenceEqual(VerifiedSealedX3DHOPKX25519PKByte));
                                    SealedX3DHOPKX25519PKBooleanChecker = (VerifiedSealedX3DHOPKX25519PKByte.SequenceEqual(VerifiedSealedDHX25519PKByte) || VerifiedSealedX3DHOPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHSPKX25519PKByte) || VerifiedSealedX3DHOPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHIKX25519PKByte));
                                    if (SealedDHX25519PKBooleanChecker==false && SealedX3DHSPKX25519PKBooleanChecker==false && SealedX3DHIKX25519PKBooleanChecker==false && SealedX3DHOPKX25519PKBooleanChecker==false) 
                                    {
                                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `Payment`(`Expiration_Date`, `ID`) VALUES (@Expiration_Date,@ID)";
                                        MySQLGeneralQuery.Parameters.Add("@Expiration_Date", MySqlDbType.DateTime).Value = DatabaseExpirationTime;
                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `Account_Lock`(`Payment_ID`, `ED25519_PK`, `Signed_ED25519_PK`) VALUES (@Payment_ID,@ED25519_PK,@Signed_ED25519_PK)";
                                        MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                                        MySQLGeneralQuery.Parameters.Add("@Signed_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedLoginED25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(LoginED25519PKByte);
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `SDH`(`Payment_ID`, `Signed_X25519_PK`, `ED25519_PK`) VALUES (@Payment_ID,@Signed_X25519_PK,@ED25519_PK)";
                                        MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                                        MySQLGeneralQuery.Parameters.Add("@Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedDHX25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedDHED25519PKByte);
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `X3SDH`(`Payment_ID`, `SPK_Signed_X25519_PK`, `SPK_ED25519_PK`, `IK_Signed_X25519_PK`, `IK_ED25519_PK`, `OPK_Signed_X25519_PK`, `OPK_ED25519_PK`) VALUES (@Payment_ID,@SPK_Signed_X25519_PK,@SPK_ED25519_PK,@IK_Signed_X25519_PK,@IK_ED25519_PK,@OPK_Signed_X25519_PK,@OPK_ED25519_PK)";
                                        MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                                        MySQLGeneralQuery.Parameters.Add("@SPK_Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedX3DHSPKX25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@SPK_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedX3DHSPKED25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@IK_Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedX3DHIKX25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@IK_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedX3DHIKED25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@OPK_Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedX3DHOPKX25519PKByte);
                                        MySQLGeneralQuery.Parameters.Add("@OPK_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedX3DHOPKED25519PKByte);
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                        MyGenerateUserForDBConnection.LoadConnection(ref ExceptionString);
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "CREATE USER '" + DBUserName + "'" + "@" + "'localhost'" + " IDENTIFIED BY " + "'" + DBUserPassword + "'";
                                        MySQLGeneralQuery.Connection = MyGenerateUserForDBConnection.GenerateDBUserConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        SodiumSecureMemory.SecureClearString(MySQLGeneralQuery.CommandText);
                                        MyGenerateUserForDBConnection.GenerateDBUserConnection.Close();
                                        MyGenerateMySQLDBConnection.LoadConnection(ref ExceptionString);
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "CREATE DATABASE " + DBName;
                                        MySQLGeneralQuery.Connection = MyGenerateMySQLDBConnection.GenerateMySQLDatabaseConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        MyGenerateMySQLDBConnection.GenerateMySQLDatabaseConnection.Close();
                                        MyGrantPermissionDBConnection.LoadConnection(ref ExceptionString);
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "GRANT ALL ON " + DBName + ".*" + " TO '" + DBUserName + "'@'localhost'";
                                        MySQLGeneralQuery.Connection = MyGrantPermissionDBConnection.GrantPermissionConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        MyGrantPermissionDBConnection.GrantPermissionConnection.Close();
                                        MyPaymentModel.Status = "Successed: You have succesfully made payment and the information have been passed back";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = MyUTC8Time.ToString();
                                        MyPaymentModel.SystemPaymentID = UniquePaymentID;
                                        CipheredDBUserNameByte = SodiumSealedPublicKeyBox.Create(DBUserNameByte, ClientECDHPKByte);
                                        CipheredDBUserPasswordByte = SodiumSealedPublicKeyBox.Create(DBUserPasswordByte, ClientECDHPKByte);
                                        CipheredDBNameByte = SodiumSealedPublicKeyBox.Create(DBNameByte, ClientECDHPKByte);
                                        MyPaymentModel.CipheredDBName = Convert.ToBase64String(CipheredDBNameByte);
                                        MyPaymentModel.CipheredDBAccountUserName = Convert.ToBase64String(CipheredDBUserNameByte);
                                        MyPaymentModel.CipheredDBAccountPassword = Convert.ToBase64String(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearString(DBName);
                                        SodiumSecureMemory.SecureClearBytes(DBNameByte);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserName);
                                        SodiumSecureMemory.SecureClearBytes(DBUserNameByte);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserNameByte);
                                        SodiumSecureMemory.SecureClearString(DBUserPassword);
                                        SodiumSecureMemory.SecureClearBytes(DBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(CipheredDBUserPasswordByte);
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return MyPaymentModel;
                                    }
                                    else 
                                    {
                                        MyPaymentModel.Status = "Error: Please make sure that the DH public keys that you submitted is not the same for all of them";
                                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                        return MyPaymentModel;
                                    }
                                }
                                else 
                                {
                                    MyPaymentModel.Status = "Error: Signed version of user login ED25519PK and unsigned version of user login ED25519PK is not match";
                                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                    return MyPaymentModel;
                                }
                            }
                            else 
                            {
                                MyPaymentModel.Status = "Error: Man in the middle spotted, are you an imposter trying to get over the ETLS?";
                                MyPaymentModel.CipheredDBName = "Error: Not Valid";
                                MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                                MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                                MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                                MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                                return MyPaymentModel;
                            }
                        }
                        else 
                        {
                            MyPaymentModel.Status = "Error: Please pass in correct Base64 Encoded String in the parameter..";
                            MyPaymentModel.CipheredDBName = "Error: Not Valid";
                            MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                            MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                            MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                            MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                            return MyPaymentModel;
                        }
                    }
                    else 
                    {
                        MyPaymentModel.Status = "Error: Please pass in correct URL Encoded String in the parameter..";
                        MyPaymentModel.CipheredDBName = "Error: Not Valid";
                        MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                        MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                        MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                        MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                        return MyPaymentModel;
                    }
                }
                else 
                {
                    MyPaymentModel.Status = "Error: The corresponding ETLS ID does not exists in the server..";
                    MyPaymentModel.CipheredDBName = "Error: Not Valid";
                    MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                    MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                    MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                    MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                    return MyPaymentModel;
                }
            }
            else
            {
                MyPaymentModel.Status = "Error: The ETLS ID mustn't be null";
                MyPaymentModel.CipheredDBName = "Error: Not Valid";
                MyPaymentModel.CipheredDBAccountPassword = "Error: Not Valid";
                MyPaymentModel.CipheredDBAccountUserName = "Error: Not Valid";
                MyPaymentModel.GMT8PaymentMadeDateTime = "Error: Not Valid";
                MyPaymentModel.SystemPaymentID = "Error: Not Valid";
                return MyPaymentModel;
            }
        }

        [HttpGet("RenewPayment")]
        public String RenewPayment(String ClientPathID, String CipheredSignedOrderID, String CipheredSignedUniquePaymentID, String SignedSignedRandomChallenge ,String CipheredSignedLoginED25519PK, String EncryptedSignedSignedLoginED25519PK, String CipheredSignedSignedSealedDHX25519PK, String CipheredSignedSealedDHED25519PK, String CipheredSignedSignedSealedX3DHSPKX25519PK, String CipheredSignedSealedX3DHSPKED25519PK, String CipheredSignedSignedSealedX3DHIKX25519PK, String CipheredSignedSealedX3DHIKED25519PK, String CipheredSignedSignedSealedX3DHOPKX25519PK, String CipheredSignedSealedX3DHOPKED25519PK)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            CryptographicSecureStrongPasswordGenerator PasswordGenerator = new CryptographicSecureStrongPasswordGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedSignedSignedRandomChallenge = "";
            Boolean DecodingSignedSignedRandomChallengeChecker = true;
            Boolean ConvertFromBase64SignedSignedRandomChallengeChecker = true;
            Byte[] SignedSignedRandomChallengeByte = new Byte[] { };
            Boolean VerifySignedRandomChallengeByteChecker = true;
            Byte[] SignedRandomChallengeByte = new Byte[] { };
            Byte[] RandomChallengeByte = new Byte[] { };
            String DecodedCipheredSignedUniquePaymentID = "";
            Boolean DecodingCipheredSignedUniquePaymentIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedUniquePaymentIDChecker = true;
            Byte[] CipheredSignedUniquePaymentIDByte = new Byte[] { };
            Boolean VerifyCipheredUniquePaymentIDByteChecker = true;
            Byte[] CipheredUniquePaymentIDByte = new Byte[] { };
            Byte[] UniquePaymentIDByte = new Byte[] { };
            String UniquePaymentID = "";
            String DecodedCipheredSignedLoginED25519PK = "";
            Boolean DecodingCipheredSignedLoginED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedLoginED25519PKChecker = true;
            Byte[] CipheredSignedLoginED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredLoginED25519PKByteChecker = true;
            Byte[] CipheredLoginED25519PKByte = new Byte[] { };
            Byte[] LoginED25519PKByte = new Byte[] { };
            String DecodedEncryptedSignedSignedLoginED25519PK = "";
            Boolean DecodingEncryptedSignedSignedLoginED25519PKChecker = true;
            Boolean ConvertFromBase64EncryptedSignedSignedLoginED25519PKChecker = true;
            Byte[] EncryptedSignedSignedLoginED25519PKByte = new Byte[] { };
            Boolean VerifyEncryptedSignedLoginED25519PKByteChecker = true;
            Byte[] EncryptedSignedLoginED25519PKByte = new Byte[] { };
            Byte[] SignedLoginED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedDHX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedDHX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedDHX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedDHX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedDHX25519PKChecker = true;
            Byte[] CipheredSignedSealedDHX25519PKByte = new Byte[] { };
            Byte[] SignedSealedDHX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedDHED25519PK = "";
            Boolean DecodingCipheredSignedSealedDHED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedDHED25519PKChecker = true;
            Byte[] CipheredSignedSealedDHED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedDHED25519PKChecker = true;
            Byte[] CipheredSealedDHED25519PKByte = new Byte[] { };
            Byte[] SealedDHED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedX3DHSPKX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedX3DHSPKX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedX3DHSPKX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedX3DHSPKX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedX3DHSPKX25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHSPKX25519PKByte = new Byte[] { };
            Byte[] SignedSealedX3DHSPKX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedX3DHSPKED25519PK = "";
            Boolean DecodingCipheredSignedSealedX3DHSPKED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedX3DHSPKED25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHSPKED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedX3DHSPKED25519PKChecker = true;
            Byte[] CipheredSealedX3DHSPKED25519PKByte = new Byte[] { };
            Byte[] SealedX3DHSPKED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedX3DHIKX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedX3DHIKX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedX3DHIKX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedX3DHIKX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedX3DHIKX25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHIKX25519PKByte = new Byte[] { };
            Byte[] SignedSealedX3DHIKX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedX3DHIKED25519PK = "";
            Boolean DecodingCipheredSignedSealedX3DHIKED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedX3DHIKED25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHIKED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedX3DHIKED25519PKChecker = true;
            Byte[] CipheredSealedX3DHIKED25519PKByte = new Byte[] { };
            Byte[] SealedX3DHIKED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSignedSealedX3DHOPKX25519PK = "";
            Boolean DecodingCipheredSignedSignedSealedX3DHOPKX25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSignedSealedX3DHOPKX25519PKChecker = true;
            Byte[] CipheredSignedSignedSealedX3DHOPKX25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSignedSealedX3DHOPKX25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHOPKX25519PKByte = new Byte[] { };
            Byte[] SignedSealedX3DHOPKX25519PKByte = new Byte[] { };
            String DecodedCipheredSignedSealedX3DHOPKED25519PK = "";
            Boolean DecodingCipheredSignedSealedX3DHOPKED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedSealedX3DHOPKED25519PKChecker = true;
            Byte[] CipheredSignedSealedX3DHOPKED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredSealedX3DHOPKED25519PKChecker = true;
            Byte[] CipheredSealedX3DHOPKED25519PKByte = new Byte[] { };
            Byte[] SealedX3DHOPKED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedOrderID = "";
            Boolean DecodingCipheredSignedOrderIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedOrderIDChecker = true;
            Byte[] CipheredSignedOrderIDByte = new Byte[] { };
            Boolean VerifyCipheredOrderIDByteChecker = true;
            Byte[] CipheredOrderIDByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            String OrderID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            Byte[] TestLoginED25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedDHX25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedX3DHSPKX25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedX3DHIKX25519PKByte = new Byte[] { };
            Byte[] VerifiedSealedX3DHOPKX25519PKByte = new Byte[] { };
            Boolean SealedDHX25519PKBooleanChecker = true;
            Boolean SealedX3DHSPKX25519PKBooleanChecker = true;
            Boolean SealedX3DHIKX25519PKBooleanChecker = true;
            Boolean SealedX3DHOPKX25519PKBooleanChecker = true;
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
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
            DateTime RandomChallengeValidDuration = new DateTime();
            TimeSpan TimeDifference = new TimeSpan();
            int PaymentIDCount = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedOrderIDChecker, ref DecodedCipheredSignedOrderID, CipheredSignedOrderID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedUniquePaymentIDChecker, ref DecodedCipheredSignedUniquePaymentID, CipheredSignedUniquePaymentID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedLoginED25519PKChecker, ref DecodedCipheredSignedLoginED25519PK, CipheredSignedLoginED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingEncryptedSignedSignedLoginED25519PKChecker, ref DecodedEncryptedSignedSignedLoginED25519PK, EncryptedSignedSignedLoginED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedDHED25519PKChecker, ref DecodedCipheredSignedSealedDHED25519PK, CipheredSignedSealedDHED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedDHX25519PKChecker, ref DecodedCipheredSignedSignedSealedDHX25519PK, CipheredSignedSignedSealedDHX25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedX3DHSPKED25519PKChecker, ref DecodedCipheredSignedSealedX3DHSPKED25519PK, CipheredSignedSealedX3DHSPKED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedX3DHSPKX25519PKChecker, ref DecodedCipheredSignedSignedSealedX3DHSPKX25519PK, CipheredSignedSignedSealedX3DHSPKX25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedX3DHIKED25519PKChecker, ref DecodedCipheredSignedSealedX3DHIKED25519PK, CipheredSignedSealedX3DHIKED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedX3DHIKX25519PKChecker, ref DecodedCipheredSignedSignedSealedX3DHIKX25519PK, CipheredSignedSignedSealedX3DHIKX25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSealedX3DHOPKED25519PKChecker, ref DecodedCipheredSignedSealedX3DHOPKED25519PK, CipheredSignedSealedX3DHOPKED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedSignedSealedX3DHOPKX25519PKChecker, ref DecodedCipheredSignedSignedSealedX3DHOPKX25519PK, CipheredSignedSignedSealedX3DHOPKX25519PK);
                    if (DecodingCipheredSignedOrderIDChecker == true && DecodingCipheredSignedUniquePaymentIDChecker==true && DecodingSignedSignedRandomChallengeChecker==true && DecodingCipheredSignedLoginED25519PKChecker == true && DecodingEncryptedSignedSignedLoginED25519PKChecker == true && DecodingCipheredSignedSealedDHED25519PKChecker == true && DecodingCipheredSignedSignedSealedDHX25519PKChecker == true && DecodingCipheredSignedSealedX3DHSPKED25519PKChecker == true && DecodingCipheredSignedSignedSealedX3DHSPKX25519PKChecker == true && DecodingCipheredSignedSealedX3DHIKED25519PKChecker == true && DecodingCipheredSignedSignedSealedX3DHIKX25519PKChecker == true && DecodingCipheredSignedSealedX3DHOPKED25519PKChecker == true && DecodingCipheredSignedSignedSealedX3DHOPKX25519PKChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedOrderIDChecker, ref CipheredSignedOrderIDByte, DecodedCipheredSignedOrderID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedUniquePaymentIDChecker, ref CipheredSignedUniquePaymentIDByte, DecodedCipheredSignedUniquePaymentID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedLoginED25519PKChecker, ref CipheredSignedLoginED25519PKByte, DecodedCipheredSignedLoginED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64EncryptedSignedSignedLoginED25519PKChecker, ref EncryptedSignedSignedLoginED25519PKByte, DecodedEncryptedSignedSignedLoginED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedDHED25519PKChecker, ref CipheredSignedSealedDHED25519PKByte, DecodedCipheredSignedSealedDHED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedDHX25519PKChecker, ref CipheredSignedSignedSealedDHX25519PKByte, DecodedCipheredSignedSignedSealedDHX25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedX3DHSPKED25519PKChecker, ref CipheredSignedSealedX3DHSPKED25519PKByte, DecodedCipheredSignedSealedX3DHSPKED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedX3DHSPKX25519PKChecker, ref CipheredSignedSignedSealedX3DHSPKX25519PKByte, DecodedCipheredSignedSignedSealedX3DHSPKX25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedX3DHIKED25519PKChecker, ref CipheredSignedSealedX3DHIKED25519PKByte, DecodedCipheredSignedSealedX3DHIKED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedX3DHIKX25519PKChecker, ref CipheredSignedSignedSealedX3DHIKX25519PKByte, DecodedCipheredSignedSignedSealedX3DHIKX25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSealedX3DHOPKED25519PKChecker, ref CipheredSignedSealedX3DHOPKED25519PKByte, DecodedCipheredSignedSealedX3DHOPKED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedSignedSealedX3DHOPKX25519PKChecker, ref CipheredSignedSignedSealedX3DHOPKX25519PKByte, DecodedCipheredSignedSignedSealedX3DHOPKX25519PK);
                        if (ConvertFromBase64CipheredSignedOrderIDChecker == true && ConvertFromBase64CipheredSignedUniquePaymentIDChecker==true && ConvertFromBase64SignedSignedRandomChallengeChecker==true && ConvertFromBase64CipheredSignedLoginED25519PKChecker == true && ConvertFromBase64EncryptedSignedSignedLoginED25519PKChecker == true && ConvertFromBase64CipheredSignedSealedDHED25519PKChecker == true && ConvertFromBase64CipheredSignedSignedSealedDHX25519PKChecker == true && ConvertFromBase64CipheredSignedSealedX3DHSPKED25519PKChecker == true && ConvertFromBase64CipheredSignedSignedSealedX3DHSPKX25519PKChecker == true && ConvertFromBase64CipheredSignedSealedX3DHIKED25519PKChecker == true && ConvertFromBase64CipheredSignedSignedSealedX3DHIKX25519PKChecker == true && ConvertFromBase64CipheredSignedSealedX3DHOPKED25519PKChecker == true && ConvertFromBase64CipheredSignedSignedSealedX3DHOPKX25519PKChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredOrderIDByteChecker, ref CipheredOrderIDByte, CipheredSignedOrderIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredUniquePaymentIDByteChecker, ref CipheredUniquePaymentIDByte, CipheredSignedUniquePaymentIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeByteChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredLoginED25519PKByteChecker, ref CipheredLoginED25519PKByte, CipheredSignedLoginED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyEncryptedSignedLoginED25519PKByteChecker, ref EncryptedSignedLoginED25519PKByte, EncryptedSignedSignedLoginED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedDHED25519PKChecker, ref CipheredSealedDHED25519PKByte, CipheredSignedSealedDHED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedDHX25519PKChecker, ref CipheredSignedSealedDHX25519PKByte, CipheredSignedSignedSealedDHX25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedX3DHSPKED25519PKChecker, ref CipheredSealedX3DHSPKED25519PKByte, CipheredSignedSealedX3DHSPKED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedX3DHSPKX25519PKChecker, ref CipheredSignedSealedX3DHSPKX25519PKByte, CipheredSignedSignedSealedX3DHSPKX25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedX3DHIKED25519PKChecker, ref CipheredSealedX3DHIKED25519PKByte, CipheredSignedSealedX3DHIKED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedX3DHIKX25519PKChecker, ref CipheredSignedSealedX3DHIKX25519PKByte, CipheredSignedSignedSealedX3DHIKX25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSealedX3DHOPKED25519PKChecker, ref CipheredSealedX3DHOPKED25519PKByte, CipheredSignedSealedX3DHOPKED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredSignedSealedX3DHOPKX25519PKChecker, ref CipheredSignedSealedX3DHOPKX25519PKByte, CipheredSignedSignedSealedX3DHOPKX25519PKByte, ClientECDSAPKByte);
                            if (VerifyCipheredOrderIDByteChecker == true && VerifyCipheredUniquePaymentIDByteChecker==true && VerifySignedRandomChallengeByteChecker==true && VerifyCipheredLoginED25519PKByteChecker == true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredOrderIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredOrderIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredOrderIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Unable to decrypt ETLS signed order ID..";
                                }
                                OrderID = Encoding.UTF8.GetString(PlainText);
                                try
                                {
                                    var captureOrderResponse = PaypalCaptureOrder.CaptureOrder(OrderID).Result;
                                    var captureOrderResult = captureOrderResponse.Result<Order>();
                                    var captureId = "";
                                    foreach (PurchaseUnit purchaseUnit in captureOrderResult.PurchaseUnits)
                                    {
                                        foreach (Capture capture in purchaseUnit.Payments.Captures)
                                        {
                                            captureId = capture.Id;
                                        }
                                    }
                                    if (captureId.CompareTo("") == 0)
                                    {
                                        return "Error: You haven't renew the payment/You have renew the payment again on the same order ID";
                                    }
                                }
                                catch
                                {
                                    return "Error: You haven't renew the payment/You have renew the payment again on the same order ID";
                                }
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredUniquePaymentIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredUniquePaymentIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredUniquePaymentIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Unable to decrypt ETLS signed unique payment ID..";
                                }
                                UniquePaymentIDByte = PlainText;
                                UniquePaymentID = Encoding.UTF8.GetString(UniquePaymentIDByte);
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredLoginED25519PKByte.Length - NonceByte.Length];
                                Array.Copy(CipheredLoginED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredLoginED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Unable to decrypt ETLS signed user sent login ed25519 pk..";
                                }
                                LoginED25519PKByte = PlainText;
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[EncryptedSignedLoginED25519PKByte.Length - NonceByte.Length];
                                Array.Copy(EncryptedSignedLoginED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(EncryptedSignedLoginED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Unable to decrypt ETLS signed user sent user signed login ed25519 pk..";
                                }
                                SignedLoginED25519PKByte = PlainText;
                                try
                                {
                                    TestLoginED25519PKByte = SodiumPublicKeyAuth.Verify(SignedLoginED25519PKByte, LoginED25519PKByte);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Unable to verify user signed login ED25519PK with corresponding sent user login ED25519PK";
                                }
                                if (LoginED25519PKByte.SequenceEqual(TestLoginED25519PKByte) == true)
                                {
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedDHED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedDHED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedDHED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS signed user sent user sealed diffie hellman digital signature public key";
                                    }
                                    SealedDHED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedDHX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedDHX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedDHX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS signed user sent user signed sealed diffie hellman key exchange public key";
                                    }
                                    SignedSealedDHX25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedX3DHSPKED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedX3DHSPKED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedX3DHSPKED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS user sent user sealed diffie hellman digital signature public key for X3DH SPK";;
                                    }
                                    SealedX3DHSPKED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedX3DHSPKX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedX3DHSPKX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedX3DHSPKX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS user sent user signed sealed diffie hellman key exchange public key for X3DH SPK";
                                    }
                                    SignedSealedX3DHSPKX25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedX3DHIKED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedX3DHIKED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedX3DHIKED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS user sent user sealed diffie hellman digital signature public key for X3DH IK";
                                    }
                                    SealedX3DHIKED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedX3DHIKX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedX3DHIKX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedX3DHIKX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS user sent user signed sealed diffie hellman key exchange public key for X3DH IK";
                                    }
                                    SignedSealedX3DHIKX25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSealedX3DHOPKED25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSealedX3DHOPKED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSealedX3DHOPKED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS user sent user sealed diffie hellman digital signature public key for X3DH OPK";
                                    }
                                    SealedX3DHOPKED25519PKByte = PlainText;
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    CipheredText = new Byte[CipheredSignedSealedX3DHOPKX25519PKByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredSignedSealedX3DHOPKX25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                    Array.Copy(CipheredSignedSealedX3DHOPKX25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                    try
                                    {
                                        PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS user sent user signed sealed diffie hellman key exchange public key for X3DH OPK";
                                    }
                                    SignedSealedX3DHOPKX25519PKByte = PlainText;
                                    try
                                    {
                                        VerifiedSealedDHX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedDHX25519PKByte, SealedDHED25519PKByte);
                                    }
                                    catch
                                    {
                                        return "Error: Unable to verify user signed sealed DH X25519PK";
                                    }
                                    try
                                    {
                                        VerifiedSealedX3DHSPKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedX3DHSPKX25519PKByte, SealedX3DHSPKED25519PKByte);
                                    }
                                    catch
                                    {
                                        return "Error: Unable to verify user signed sealed X3DH SPK X25519PK";
                                    }
                                    try
                                    {
                                        VerifiedSealedX3DHIKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedX3DHIKX25519PKByte, SealedX3DHIKED25519PKByte);
                                    }
                                    catch
                                    {
                                        return "Error: Unable to verify user signed sealed X3DH IK X25519PK";
                                    }
                                    try
                                    {
                                        VerifiedSealedX3DHOPKX25519PKByte = SodiumPublicKeyAuth.Verify(SignedSealedX3DHOPKX25519PKByte, SealedX3DHOPKED25519PKByte);
                                    }
                                    catch
                                    {
                                        return "Error: Unable to verify user signed sealed X3DH OPK X25519PK";
                                    }
                                    SealedDHX25519PKBooleanChecker = (VerifiedSealedDHX25519PKByte.SequenceEqual(VerifiedSealedX3DHSPKX25519PKByte) || VerifiedSealedDHX25519PKByte.SequenceEqual(VerifiedSealedX3DHIKX25519PKByte) || VerifiedSealedDHX25519PKByte.SequenceEqual(VerifiedSealedX3DHOPKX25519PKByte));
                                    SealedX3DHSPKX25519PKBooleanChecker = (VerifiedSealedX3DHSPKX25519PKByte.SequenceEqual(VerifiedSealedDHX25519PKByte) || VerifiedSealedX3DHSPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHIKX25519PKByte) || VerifiedSealedX3DHSPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHOPKX25519PKByte));
                                    SealedX3DHIKX25519PKBooleanChecker = (VerifiedSealedX3DHIKX25519PKByte.SequenceEqual(VerifiedSealedDHX25519PKByte) || VerifiedSealedX3DHIKX25519PKByte.SequenceEqual(VerifiedSealedX3DHSPKX25519PKByte) || VerifiedSealedX3DHIKX25519PKByte.SequenceEqual(VerifiedSealedX3DHOPKX25519PKByte));
                                    SealedX3DHOPKX25519PKBooleanChecker = (VerifiedSealedX3DHOPKX25519PKByte.SequenceEqual(VerifiedSealedDHX25519PKByte) || VerifiedSealedX3DHOPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHSPKX25519PKByte) || VerifiedSealedX3DHOPKX25519PKByte.SequenceEqual(VerifiedSealedX3DHIKX25519PKByte));
                                    if (SealedDHX25519PKBooleanChecker == false && SealedX3DHSPKX25519PKBooleanChecker == false && SealedX3DHIKX25519PKBooleanChecker == false && SealedX3DHOPKX25519PKBooleanChecker == false)
                                    {
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
                                                DatabaseExpirationTime = DatabaseExpirationTime.AddDays(30);
                                            }
                                            else
                                            {
                                                DatabaseExpirationTime = MyUTC8Time.AddDays(30);
                                            }
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
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
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
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    MySQLGeneralQuery.ExecuteNonQuery();
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "UPDATE `Payment` SET `Expiration_Date`=@Expiration_Date WHERE `ID`=@ID";
                                                    MySQLGeneralQuery.Parameters.Add("@Expiration_Date", MySqlDbType.DateTime).Value = DatabaseExpirationTime;
                                                    MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    MySQLGeneralQuery.ExecuteNonQuery();
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "UPDATE `Account_Lock` SET `ED25519_PK`=@ED25519_PK,`Signed_ED25519_PK`=@Signed_ED25519_PK WHERE `Payment_ID`=@Payment_ID";
                                                    MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                    MySQLGeneralQuery.Parameters.Add("@Signed_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedLoginED25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(LoginED25519PKByte);
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    MySQLGeneralQuery.ExecuteNonQuery();
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "UPDATE `SDH` SET `Signed_X25519_PK`=@Signed_X25519_PK,`ED25519_PK`=@ED25519_PK WHERE `Payment_ID`=@Payment_ID";
                                                    MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                    MySQLGeneralQuery.Parameters.Add("@Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedDHX25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedDHED25519PKByte);
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    MySQLGeneralQuery.ExecuteNonQuery();
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "UPDATE `X3SDH` SET `SPK_Signed_X25519_PK`=@SPK_Signed_X25519_PK,`SPK_ED25519_PK`=@SPK_ED25519_PK,`IK_Signed_X25519_PK`=@IK_Signed_X25519_PK,`IK_ED25519_PK`=@IK_ED25519_PK,`OPK_Signed_X25519_PK`=@OPK_Signed_X25519_PK,`OPK_ED25519_PK`=@OPK_ED25519_PK WHERE `Payment_ID`=@Payment_ID";
                                                    MySQLGeneralQuery.Parameters.Add("@Payment_ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                    MySQLGeneralQuery.Parameters.Add("@SPK_Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedX3DHSPKX25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@SPK_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedX3DHSPKED25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@IK_Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedX3DHIKX25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@IK_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedX3DHIKED25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@OPK_Signed_X25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SignedSealedX3DHOPKX25519PKByte);
                                                    MySQLGeneralQuery.Parameters.Add("@OPK_ED25519_PK", MySqlDbType.Text).Value = Convert.ToBase64String(SealedX3DHOPKED25519PKByte);
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    MySQLGeneralQuery.ExecuteNonQuery();
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    return "Successed: You have succesfully renew the payment";
                                                }
                                                else
                                                {
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    MySQLGeneralQuery.ExecuteNonQuery();
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
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
                                            return "Error: PaymentID does not exists in the system";
                                        }
                                    }
                                    else 
                                    {
                                        return "Error: Please make sure that the DH public keys that you submitted is not the same for all of them";
                                    }
                                }
                                else
                                {
                                    return "Error: Signed version of user login ED25519PK and unsigned version of user login ED25519PK is not match";
                                }
                            }
                            else
                            {
                                return "Error: Man in the middle spotted, are you an imposter trying to get over the ETLS?";
                            }
                        }
                        else
                        {
                            return "Error: Please pass in correct Base64 Encoded String in the parameter..";
                        }
                    }
                    else
                    {
                        return "Error: Please pass in correct URL Encoded String in the parameter..";
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
