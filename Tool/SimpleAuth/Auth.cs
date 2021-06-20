using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Management;
using DevExpress.XtraEditors;
using System.IO;
using Newtonsoft.Json;
using SimpleAuth.Properties;

namespace SimpleAuth
{
    class Auth
    {
        public static string URL = "http://127.0.0.1/auth/main.php";
        public enum FREE_MODE
        {
            ENABLED,
            DISABLED,
            INTERNETERROR,
            UNKNOWNERROR
        }
        public enum SimpleAuthLogin
        {
            USERNAMENOTFOUND,
            PASSWORDNOTFOUND,
            EMPTYPARAMS,
            INVALIDPARAMS,
            HWIDUPDATED,
            HWIDERROR,
            INTERNETERROR,
            UNKNOWNERROR,
            SUCCESS,
            BANNED
        }
        public enum SimpleAuthRegister
        {
            usernameExists,
            emailExists,
            passwordNoMatch,
            EMPTYPARAMS,
            INVALIDPARAMS,
            InvalidToken,
            INTERNETERROR,
            UNKNOWNERROR,
            SUCCESS
        }

        internal static string USERNAME(TextEdit textEdit1)
        {
            throw new NotImplementedException();
        }
        public static string get_user_level(int userlevel)
        {
            switch (userlevel)
            {
                case 1:return "Admin"; break;
                case 0:return "User"; break;
                default: return "User"; break;
            }
        }
        public static FREE_MODE CHECK_FOR_FREEMODE()
        {
            try
            {
                using (var client = new WebClient { Proxy = null })
                {
                    var values = new NameValueCollection();
                    values["type"] = "freemode";
                    var response = client.UploadValues(URL, values);
                    var responseString = Encoding.Default.GetString(response);
                    dynamic json = JsonConvert.DeserializeObject(responseString);
                    switch ((string)json.freemdoe_status)
                    {
                        case "1":
                            Settings.Default.username = (string)json.username;
                            Settings.Default.password = (string)json.password;
                            Settings.Default.email = (string)json.email;
                            Settings.Default.user_level = get_user_level((int)json.user_level);
                            Settings.Default.Save();
                            return FREE_MODE.ENABLED;//freemode = true
                        case "0":
                            return FREE_MODE.DISABLED;//freemode = false
                        default:
                            return FREE_MODE.UNKNOWNERROR;//unknown error
                    }

                }
            }
            catch
            {
                return FREE_MODE.INTERNETERROR;//internet / server error
            }
        }
        public static SimpleAuthLogin Login(string username, string password)
        {
            try
            {
                using (var client = new WebClient { Proxy = null })
                {
                    var values = new NameValueCollection();
                    values["type"] = "login";
                    values["username"] = username;
                    values["password"] = password;
                    values["hwid"] = HWID.getUniqueID();
                    var response = client.UploadValues(URL, values);
                    var responseString = Encoding.Default.GetString(response);
                    dynamic json = JsonConvert.DeserializeObject(responseString);
                    switch ((string)json.result)
                    {
                        case "Error [0x00000001]"://username
                            return SimpleAuthLogin.USERNAMENOTFOUND;
                        case "Error [0x00000002]"://password
                            return SimpleAuthLogin.PASSWORDNOTFOUND;
                        case "Error [0x00000003]"://hwid no match
                            return SimpleAuthLogin.HWIDERROR;
                        case "Success [0x00001337]"://login success
                            Settings.Default.username = (string)json.username;
                            Settings.Default.password = (string)json.password;
                            Settings.Default.email = (string)json.email;
                            Settings.Default.user_level = get_user_level((int)json.user_level);
                            Settings.Default.Save();
                            return SimpleAuthLogin.SUCCESS;
                        case "Error [0x00000004]"://banned
                            return SimpleAuthLogin.BANNED;
                        case "Error [0x00000005]"://hwid updated
                            return SimpleAuthLogin.HWIDUPDATED;
                        case "Error [0x00000006]"://empty param
                            return SimpleAuthLogin.EMPTYPARAMS;
                        case "Error [0x00000007]"://invalid param
                            return SimpleAuthLogin.INVALIDPARAMS;
                        default:
                            return SimpleAuthLogin.UNKNOWNERROR;//unknown error
                    }
                }
            }
            catch
            {
                return SimpleAuthLogin.INTERNETERROR;//internet or server error
            }
        }
        public static SimpleAuthRegister Register(string username, string password, string email, string token)
        {
            try
            {
                using (var client = new WebClient { Proxy = null })
                {
                    var values = new NameValueCollection();
                    values["type"] = "register";
                    values["username"] = username;
                    values["password"] = password;
                    values["email"] = email;
                    values["token"] = token;
                    values["hwid"] = HWID.getUniqueID();
                    var response = client.UploadValues(URL, values);
                    var responseString = Encoding.Default.GetString(response);
                    dynamic json = JsonConvert.DeserializeObject(responseString);
                    switch ((string)json.result)
                    {
                        case "Error [0x00000001]"://username
                            return SimpleAuthRegister.usernameExists;
                        case "Error [0x00000002]"://email
                            return SimpleAuthRegister.emailExists;
                        case "Success [0x00001337]"://login success
                            return SimpleAuthRegister.SUCCESS;
                        case "Error [0x00000006]"://empty param
                            return SimpleAuthRegister.EMPTYPARAMS;
                        case "Error [0x00000007]"://invalid param
                            return SimpleAuthRegister.INVALIDPARAMS;
                        case "Error [0x00000004]"://invalid param
                            return SimpleAuthRegister.InvalidToken;
                        default:
                            return SimpleAuthRegister.UNKNOWNERROR;//unknown error
                    }
                }
            }
            catch
            {
                return SimpleAuthRegister.INTERNETERROR;//internet or server error
            }
        }
        class HWID
        {
            public static string getUniqueID()
            {
                string drive = "C";
                if (drive == string.Empty)
                {
                    foreach (DriveInfo compDrive in DriveInfo.GetDrives())
                    {
                        if (compDrive.IsReady)
                        {
                            drive = compDrive.RootDirectory.ToString();
                            break;
                        }
                    }
                }
                if (drive.EndsWith(":\\"))
                {
                    drive = drive.Substring(0, drive.Length - 2);
                }
                string volumeSerial = getVolumeSerial(drive);
                string cpuID = getCPUID();
                return cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 4);
            }
            static string getVolumeSerial(string drive)
            {
                ManagementObject disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
                disk.Get();
                string volumeSerial = disk["VolumeSerialNumber"].ToString();
                disk.Dispose();
                return volumeSerial;
            }
            public static string PCUSERNAME()
            {
                return Environment.UserName;
            }
            public static string PCNAME()
            {
                return Environment.MachineName;
            }
            static string getCPUID()
            {
                string cpuInfo = "";
                ManagementClass managClass = new ManagementClass("win32_processor");
                ManagementObjectCollection managCollec = managClass.GetInstances();
                foreach (ManagementObject managObj in managCollec)
                {
                    if (cpuInfo == "")
                    {
                        cpuInfo = managObj.Properties["processorID"].Value.ToString();
                        break;
                    }
                }
                return cpuInfo;
            }
        }
    }
}
