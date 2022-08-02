using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace ProfileChecker.Authentication
{
    internal class CryptoUtils
    {
        private static readonly string _key = "G3gUa6OUkGmAC0AB";
        private static readonly string _key2 = "BE6B9462ACCE477EBEB34810BD20B561";

        private readonly char[] _baseChars = new char[62]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private readonly UTF8Encoding _encoding;

        public string _message;

        public Dictionary<string, string> Dic = new Dictionary<string, string>();

        public CryptoUtils()
        {
            _message = "Unknow";
            _encoding = new UTF8Encoding();
        }

        private string DataDecrypt(string cipherText)
        {
            try
            {
                var result = string.Empty;

                var rijndaelManaged = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 256,
                    BlockSize = 128,
                    Key = Encoding.ASCII.GetBytes(_key2)
                };

                var key = Base64Decode(cipherText.Substring(0, 24));
                cipherText = cipherText.Substring(24);
                rijndaelManaged.IV = Encoding.ASCII.GetBytes(key);

                var bytes = rijndaelManaged.CreateDecryptor().TransformFinalBlock(Convert.FromBase64String(cipherText),
                    0, Convert.FromBase64String(cipherText).Length);
                result = Base64Decode(_encoding.GetString(bytes));

                rijndaelManaged.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                _message = ex.Message;
            }

            return string.Empty;
        }

        private string DataEncrypt(string plainText)
        {
            try
            {
                var result = string.Empty;

                var rijndaelManaged = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 256,
                    BlockSize = 128,
                    Key = Encoding.ASCII.GetBytes(_key2)
                };

                rijndaelManaged.IV = Encoding.ASCII.GetBytes(RandomString(16));
                plainText = Base64Encode(plainText);
                result = Convert.ToBase64String(rijndaelManaged.CreateEncryptor()
                    .TransformFinalBlock(_encoding.GetBytes(plainText), 0, plainText.Length));
                result = Base64Encode(Encoding.ASCII.GetString(rijndaelManaged.IV)) + result;

                rijndaelManaged.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                _message = ex.Message;
            }

            return string.Empty;
        }

        private string LicenseDecrypt(string cipherText)
        {
            try
            {
                var result = string.Empty;

                var rijndaelManaged = new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 256,
                    BlockSize = 128,
                    Key = Encoding.ASCII.GetBytes(_key2)
                };

                var key = Base64Decode(cipherText.Substring(0, 24));
                cipherText = cipherText.Substring(24);
                rijndaelManaged.IV = Encoding.ASCII.GetBytes(key);

                var bytes = rijndaelManaged.CreateDecryptor().TransformFinalBlock(Convert.FromBase64String(cipherText),
                    0, Convert.FromBase64String(cipherText).Length);
                result = Base64Decode(_encoding.GetString(bytes));

                rijndaelManaged.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                _message = ex.Message;
            }

            return string.Empty;
        }

        internal string RandomString(int length)
        {
            var array = new char[length];
            var array2 = new byte[length];

            using (var rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rNGCryptoServiceProvider.GetBytes(array2);
            }

            for (var i = 0; i < array.Length; i++)
            {
                var num = array2[i] % _baseChars.Length;
                array[i] = _baseChars[num];
            }

            return new string(array);
        }

        internal static string Base64Decode(string text)
        {
            try
            {
                var bytes = Convert.FromBase64String(text);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        internal static string Base64Encode(string text)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string CreateHash(string input, int length)
        {
            var hashBytes = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in hashBytes) sb.Append(b.ToString("X2"));

            if (length > sb.Length) return sb.ToString();

            return sb.ToString(0, length);
        }

        public string CreateMD5(string input)
        {
            using (var mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                var sb = new StringBuilder();
                var hashBytes = Encoding.UTF8.GetBytes(input);
                hashBytes = mD5CryptoServiceProvider.ComputeHash(hashBytes);

                for (var i = 0; i < hashBytes.Length; i++) sb.Append(hashBytes[i].ToString("X2"));
                return sb.ToString();
            }
        }

        public string Decrypt(string cipherText)
        {
            return DataDecrypt(cipherText);
        }

        public string Encrypt(string plainText)
        {
            return DataEncrypt(plainText);
        }

        public string LicDecrypt(string cipherText)
        {
            return LicenseDecrypt(cipherText);
        }
    }
}
