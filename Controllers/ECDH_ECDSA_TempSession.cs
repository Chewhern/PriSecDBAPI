using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASodium;
using System.Text;
using System.IO;
using System.Web;
using System.Runtime.InteropServices;
using PriSecDBAPI.Model;

namespace PriSecDBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECDH_ECDSA_TempSession : ControllerBase
    {

        [HttpGet("byID")]
        public ECDH_ECDSA_Models TempSession(String ClientPathID) 
        {
            ECDH_ECDSA_Models MyECDH_ECDSA_Models = new ECDH_ECDSA_Models();
            StringBuilder MyStringBuilder = new StringBuilder();
            Byte[] ServerECDSAPK = new Byte[] { };
            Byte[] ServerECDHSPK = new Byte[] { };
            RevampedKeyPair ServerECDHKeyPair = SodiumPublicKeyBox.GenerateRevampedKeyPair();
            RevampedKeyPair ServerECDSAKeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
            int CallCount = 0;
            String Path = "{Path to ETLS Session folder}";
            StreamWriter MyStreamWriter;
            StreamReader MyStreamReader;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                Path += ClientPathID;
                if (Directory.Exists(Path))
                {
                    if (System.IO.File.Exists(Path +"/"+"CallCount.txt"))
                    {
                        MyStreamReader = new StreamReader(Path +"/" + "CallCount.txt");
                        CallCount = int.Parse(MyStreamReader.ReadLine());
                        MyStreamReader.Close();
                        MyStreamReader.Dispose();
                        if (CallCount < 3)
                        {
                            ServerECDSAPK = ServerECDSAKeyPair.PublicKey;
                            ServerECDHSPK = SodiumPublicKeyAuth.Sign(ServerECDHKeyPair.PublicKey, ServerECDSAKeyPair.PrivateKey);
                            MyECDH_ECDSA_Models.ECDH_SPK_Base64String = Convert.ToBase64String(ServerECDHSPK);
                            MyECDH_ECDSA_Models.ECDSA_PK_Base64String = Convert.ToBase64String(ServerECDSAPK);
                            MyECDH_ECDSA_Models.ID_Checker_Message = "You still can use the exact same client ID...";
                            if (Directory.Exists(Path +  "/"))
                            {
                                System.IO.File.WriteAllBytes(Path +  "/" + "ECDHSK.txt", ServerECDHKeyPair.PrivateKey);
                                CallCount += 1;
                                MyStreamWriter = new StreamWriter(Path + "/" + "CallCount.txt");
                                MyStreamWriter.WriteLine(CallCount);
                                MyStreamWriter.Close();
                            }
                            else
                            {
                                Directory.CreateDirectory(Path +  "/");
                                System.IO.File.WriteAllBytes(Path +  "/" + "ECDHSK.txt", ServerECDHKeyPair.PrivateKey);
                                CallCount += 1;
                                MyStreamWriter = new StreamWriter(Path +  "/" + "CallCount.txt");
                                MyStreamWriter.WriteLine(CallCount);
                                MyStreamWriter.Close();
                            }
                        }
                        else
                        {
                            MyECDH_ECDSA_Models.ECDH_SPK_Base64String = "None";
                            MyECDH_ECDSA_Models.ECDSA_PK_Base64String = "None";
                            MyECDH_ECDSA_Models.ID_Checker_Message = "You can no longer use the exact same client ID....";
                        }
                    }
                }
                else 
                {
                    Directory.CreateDirectory(Path);
                    ServerECDSAPK = ServerECDSAKeyPair.PublicKey;
                    ServerECDHSPK = SodiumPublicKeyAuth.Sign(ServerECDHKeyPair.PublicKey, ServerECDSAKeyPair.PrivateKey);
                    MyECDH_ECDSA_Models.ECDH_SPK_Base64String = Convert.ToBase64String(ServerECDHSPK);
                    MyECDH_ECDSA_Models.ECDSA_PK_Base64String = Convert.ToBase64String(ServerECDSAPK);
                    if (Directory.Exists(Path +  "/"))
                    {
                        System.IO.File.WriteAllBytes(Path +  "/" + "ECDHSK.txt", ServerECDHKeyPair.PrivateKey);
                        CallCount += 1;
                        MyStreamWriter = new StreamWriter(Path + "/" + "CallCount.txt");
                        MyStreamWriter.WriteLine(CallCount);
                        MyStreamWriter.Close();
                    }
                    else 
                    {
                        Directory.CreateDirectory(Path +  "/");
                        System.IO.File.WriteAllBytes(Path +  "/" + "ECDHSK.txt", ServerECDHKeyPair.PrivateKey);
                        CallCount += 1;
                        MyStreamWriter = new StreamWriter(Path +  "/" + "CallCount.txt");
                        MyStreamWriter.WriteLine(CallCount);
                        MyStreamWriter.Close();
                    }
                    MyECDH_ECDSA_Models.ID_Checker_Message = "You have an exact client ID great~";
                }
            }
            else 
            {
                MyECDH_ECDSA_Models.ECDH_SPK_Base64String = "None";
                MyECDH_ECDSA_Models.ECDSA_PK_Base64String = "None";
                MyECDH_ECDSA_Models.ID_Checker_Message = "Please provide an ID";
            }
            ServerECDHKeyPair.Clear();
            ServerECDSAKeyPair.Clear();
            GCHandle MyGeneralGCHandle = GCHandle.Alloc(ClientPathID, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientPathID.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(Path, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), Path.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ServerECDSAPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ServerECDSAPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ServerECDHSPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ServerECDHSPK.Length);
            MyGeneralGCHandle.Free();
            return MyECDH_ECDSA_Models;
        }

        [HttpGet("DeleteByClientCryptographicID")]
        public String DeleteClientCryptographicSessionPathID(String ClientPathID, String ValidationData) 
        {
            String Status = "";
            String Path = "/Projects/PriSecDBAPI/TempSession/";
            Byte[] ClientECDSAPK = new Byte[] { };
            Path += ClientPathID;
            Boolean DecodingBoolean = true;
            Boolean VerifyBoolean = true;
            Boolean ConvertFromBase64Boolean = true;
            String DecodedValidationDataString = "";
            Byte[] ValidationDataByte = new Byte[] { };
            Byte[] ValidatedDataByte = new Byte[] { };
            if (Directory.Exists(Path))
            {
                ClientECDSAPK = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                try 
                {
                    if (ValidationData.Contains("+")) 
                    {
                        DecodedValidationDataString = ValidationData;    
                    }
                    else 
                    {
                        DecodedValidationDataString = HttpUtility.UrlDecode(ValidationData);
                    }
                }
                catch 
                {
                    DecodingBoolean = false;
                }
                if (DecodingBoolean == true) 
                {
                    try 
                    {
                        ValidationDataByte = Convert.FromBase64String(DecodedValidationDataString);
                    }
                    catch 
                    {
                        ConvertFromBase64Boolean = false;
                    }
                    if (ConvertFromBase64Boolean == true) 
                    {
                        try 
                        {
                            ValidatedDataByte = SodiumPublicKeyAuth.Verify(ValidationDataByte, ClientECDSAPK);
                        }
                        catch 
                        {
                            VerifyBoolean = false;
                        }
                        if (VerifyBoolean == true) 
                        {
                            try
                            {
                                Directory.Delete(Path, true);
                                Status = "Successfully deleted....";
                            }
                            catch (Exception ex)
                            {
                                Status = "Error: Something went wrong... Here's the error: " + ex.ToString();
                            }
                        }
                        else 
                        {
                            Status = "Error: Are you an imposter? ECDSA(ED25519) public key doesn't match";
                        }
                    }
                    else 
                    {
                        Status = "Error: You didn't pass in correct Base 64 encoded parameter value..";
                    }
                }
                else 
                {
                    Status = "Error: You didn't pass in correct URL encoded parameter value..";
                }
            }
            else 
            {
                Status = "Error: Client Path does not exists or deleted...";
            }
            GCHandle MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ValidationData, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ValidationData.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(DecodedValidationDataString, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedValidationDataString.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ValidationDataByte, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ValidationDataByte.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ValidatedDataByte, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ValidatedDataByte.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ClientPathID, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientPathID.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(Path, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), Path.Length);
            MyGeneralGCHandle.Free();
            return Status;
        }

        [HttpGet("ByHandshake")]
        public String ECDHE_ECDSA_Session(String ClientPathID, String SECDHPK,String ECDSAPK) 
        {
            String SessionStatus = "";
            Byte[] ECDHSK = new Byte[] { };
            Byte[] ClientECDHPK = new Byte[] { };
            Byte[] ClientECDSAPK = new Byte[] { };
            Byte[] ClientSignedECDHPK = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            String Path = "/Projects/PriSecDBAPI/TempSession/";
            Path += ClientPathID;
            String DecodedSECDHPK = "";
            String DecodedECDSAPK = "";
            Boolean URLDecodeChecker1 = true;
            Boolean URLDecodeChecker2 = true;
            Boolean Base64StringChecker1 = true;
            Boolean Base64StringChecker2 = true;
            Boolean VerifyBoolean = true;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ECDHSK = System.IO.File.ReadAllBytes(Path+"/"+"ECDHSK.txt");
                    try 
                    {
                        if (SECDHPK.Contains("+")) 
                        {
                            DecodedSECDHPK = SECDHPK;
                        }
                        else 
                        {
                            DecodedSECDHPK = HttpUtility.UrlDecode(SECDHPK);
                        }
                    }
                    catch 
                    {
                        URLDecodeChecker1 = false;
                    }
                    try 
                    {
                        if (ECDSAPK.Contains("+")) 
                        {
                            DecodedECDSAPK = ECDSAPK;
                        }
                        else 
                        {
                            DecodedECDSAPK = HttpUtility.UrlDecode(ECDSAPK);
                        }
                    }
                    catch 
                    {
                        URLDecodeChecker2 = false;
                    }
                    if(URLDecodeChecker1==true && URLDecodeChecker2 == true) 
                    {
                        try 
                        {
                            ClientSignedECDHPK = Convert.FromBase64String(DecodedSECDHPK);
                        }
                        catch 
                        {
                            Base64StringChecker1 = false;
                        }
                        try 
                        {
                            ClientECDSAPK = Convert.FromBase64String(DecodedECDSAPK);
                        }
                        catch 
                        {
                            Base64StringChecker2 = false;
                        }
                        if(Base64StringChecker1==true && Base64StringChecker2 == true) 
                        {
                            try 
                            {
                                ClientECDHPK = SodiumPublicKeyAuth.Verify(ClientSignedECDHPK,ClientECDSAPK);
                            }
                            catch 
                            {
                                VerifyBoolean = false;
                            }
                            if (VerifyBoolean == true) 
                            {
                                SharedSecret = SodiumScalarMult.Mult(ECDHSK, ClientECDHPK);
                                System.IO.File.WriteAllBytes(Path + "/" + "SharedSecret.txt", SharedSecret);
                                System.IO.File.WriteAllBytes(Path + "/" + "ClientECDHPK.txt", ClientECDHPK);
                                System.IO.File.WriteAllBytes(Path + "/" + "ClientECDSAPK.txt", ClientECDSAPK);
                                System.IO.File.Delete(Path + "/" + "ECDHSK.txt");
                                SessionStatus = "Successed... You have established an ephemeral shared secret with the server with man in the middle prevention...";
                            }
                            else 
                            {
                                SessionStatus = "Error: Are you an imposter trying to mimic the session?(ED25519 keypair not match)";
                            }
                        }
                        else 
                        {
                            SessionStatus = "Error: You didn't pass in correct Base64 format paramter values";
                        }
                    }
                    else 
                    {
                        SessionStatus = "Error: You didn't pass in correct URL encoded parameter value...";
                    }
                }
                else 
                {
                    SessionStatus = "Error: You don't pass in correct ClientSessionID";
                }
            }
            else 
            {
                SessionStatus = "Error: You don't specify the client path ID";
            }
            GCHandle MyGeneralGCHandle = GCHandle.Alloc(ECDHSK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ECDHSK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ClientSignedECDHPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientSignedECDHPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(SECDHPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SECDHPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ECDSAPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ECDSAPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(DecodedSECDHPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSECDHPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(DecodedECDSAPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedECDSAPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ClientPathID, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientPathID.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(Path, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), Path.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(ClientECDHPK, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDHPK.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
            MyGeneralGCHandle.Free();
            return SessionStatus;
        }

        [HttpGet("BySharedSecret")]
        public String CheckSharedSecret(String ClientPathID,String CipheredData , String Nonce) 
        {
            String CheckSharedSecretStatus = "";
            Byte[] SharedSecret = new Byte[] { };
            Byte[] CipheredDataByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] TestDecryptedByte = new Byte[] { };
            String Path = "/Projects/PriSecDBAPI/TempSession/";
            Path += ClientPathID;
            String DecodedCipheredData = "";
            String DecodedNonce = "";
            Boolean URLDecodeChecker1 = true;
            Boolean URLDecodeChecker2 = true;
            Boolean Base64StringChecker1 = true;
            Boolean Base64StringChecker2 = true;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    try
                    {
                        if (CipheredData.Contains("+"))
                        {
                            DecodedCipheredData = CipheredData;
                        }
                        else
                        {
                            DecodedCipheredData = HttpUtility.UrlDecode(CipheredData);
                        }
                    }
                    catch
                    {
                        URLDecodeChecker1 = false;
                    }
                    try
                    {
                        if (Nonce.Contains("+"))
                        {
                            DecodedNonce = Nonce;
                        }
                        else
                        {
                            DecodedNonce = HttpUtility.UrlDecode(Nonce);
                        }
                    }
                    catch
                    {
                        URLDecodeChecker2 = false;
                    }
                    if(URLDecodeChecker1 == true && URLDecodeChecker2 == true) 
                    {
                        try 
                        {
                            CipheredDataByte = Convert.FromBase64String(DecodedCipheredData);
                        }
                        catch 
                        {
                            Base64StringChecker1 = false;
                        }
                        try 
                        {
                            NonceByte = Convert.FromBase64String(DecodedNonce);
                        }
                        catch 
                        {
                            Base64StringChecker2 = false;
                        }
                        if(Base64StringChecker1==true && Base64StringChecker2 == true) 
                        {
                            try 
                            {
                                TestDecryptedByte = SodiumSecretBox.Open(CipheredDataByte,NonceByte,SharedSecret);
                                CheckSharedSecretStatus = "Successed... The shared secret matched"; 
                            }
                            catch 
                            {
                                CheckSharedSecretStatus = "Error: The shared secret does not match";
                            }
                        }
                        else 
                        {
                            CheckSharedSecretStatus = "Error: You didn't pass in correct Base64 format paramter values";
                        }
                    }
                    else 
                    {
                        CheckSharedSecretStatus = "Error: You didn't pass in correct URL encoded parameter value...";
                    }
                }
                else
                {
                    CheckSharedSecretStatus= "Error: You don't pass in correct ClientSessionID";
                }
            }
            else
            {
                CheckSharedSecretStatus = "Error: You don't specify the client path ID";
            }
            GCHandle MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
            MyGeneralGCHandle.Free();
            return CheckSharedSecretStatus;
        }
    }
}
