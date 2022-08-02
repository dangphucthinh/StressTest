using System;
using ProfileChecker.Authentication;

public class Credential
{
    public Credential()
    {
        _cryptoUtils = new CryptoUtils();
    }

    private readonly CryptoUtils _cryptoUtils;

    private string _jwtBearer;
    private string _refreshTit;

    public string Username { get; set; }
    public string Password { get; set; }

    public LoginRequest LoginRequest
    {
        get
        {
            return new LoginRequest
            {
                Username = Username,
                Password = Password,
                //Key = Utils.GetLicensekey(),
                //DeviceId = Utils.GetDeviceId(),
            };
        }
    }
    public RefreshTokenRequest RefreshRequest
    {
        get
        {
            return new RefreshTokenRequest
            {
                RefreshToken = _refreshTit
            };
        }
    }

    public string JwtBearer => _jwtBearer;
    public string JwtRefresh => _refreshTit;

    public DateTime TokenCheckin;


    public void Login(string response)
    {
        //var rawModel = _cryptoUtils.Decrypt(response.Substring(1, response.Length - 2));
        var keys = Utils.Deserialize<CredenialKeys>(response);
        _jwtBearer = keys.token;
        _refreshTit = keys.refreshToken;
        TokenCheckin = DateTime.UtcNow;
        keys = null;
    }

    class CredenialKeys
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; }
}
