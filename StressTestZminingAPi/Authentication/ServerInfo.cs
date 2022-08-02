using System.IO;
using System.Web;
using ProfileChecker;
using ProfileChecker.Authentication;
using Utils = ProfileChecker.Authentication.Utils;

public static class ServerInfo
{
    public static string AccessToken = "";
    public static string CUSTOMER_API = "http://203.205.40.149:6060/us-customer-api";
    public static string API_v1 = "/api/v1";

    public static string SERVER_ADDRESS = "203.205.40.149:6060/";
    public static string BASE_URL = "http://" + SERVER_ADDRESS;
    public static string CAMPAIGN_API_URL = BASE_URL + "us-running-campaign-api/api/v1/";



    private static Credential _credential = new Credential();
    public static Credential Credential
    {
        get
        {
            if (_credential == null)
                _credential = new Credential();
            return _credential;
        }
    }

    public static void LoginByAdmin()
    {
        _credential.Username = "admin@vnitsolutions.com";
        //_credential.Password = "GaWBlMJk@R4*NULC8260Uz6H";
        _credential.Password = "CHosting@123456";
        //_credential.Username = Global.AdminUsername;
        //_credential.Password = Global.AdminPassword;
        Login();
    }

    public static void RefreshToken()
    {
        string tenantData = null;
        var cryptoUtils = new CryptoUtils();
        var licenseRequestParams = cryptoUtils.Encrypt(Utils.Serialize(_credential.RefreshRequest));
        var content = HttpUtility.UrlEncode(licenseRequestParams);
        var licenseRequestURL = CUSTOMER_API + API_v1 + "Authentication/RefreshToken?c=" + content;
        var getCurrentTenantRequest = HttpClientEx.BuildRequest("POST", licenseRequestURL);
        getCurrentTenantRequest.ContentLength = 0;// data.Length;
        getCurrentTenantRequest.Expect = "application/json";

        using (var response = getCurrentTenantRequest.GetResponse())
        using (var responseStream = response.GetResponseStream())
        using (var reader = new StreamReader(responseStream))
        {
            tenantData = reader.ReadToEnd();
        }

        _credential.Login(tenantData);
    }

    static void Login()
    {
        try
        {
            string tenantData = null;
            var loginRequest = Utils.Serialize(_credential.LoginRequest);
            byte[] data = Utils.GetBytes(loginRequest);

            var getCurrentTenantRequest = HttpClientEx.BuildRequest("POST", CUSTOMER_API + API_v1 + "/auth/gettoken");
            getCurrentTenantRequest.ContentLength = data.Length;// data.Length;
            getCurrentTenantRequest.Expect = "application/json";

            using (var streamWriter = new StreamWriter(getCurrentTenantRequest.GetRequestStream()))
                streamWriter.Write(loginRequest);

            using (var response = getCurrentTenantRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                tenantData = reader.ReadToEnd();
            }
            _credential.Login(tenantData);
        }
        catch (Exception ex)
        {
            throw new Exception("Incorrect username or password!");
        }

    }
}