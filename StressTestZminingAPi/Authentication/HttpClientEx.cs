using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProfileChecker.Authentication
{
    internal class HttpClientEx
    {
        public bool AllowAutoRedirect = true;

        public CookieContainer CookieContainer = new CookieContainer();

        public HttpStatusCode HttpStatusCode;

        public string Message = "Unknow Error!";

        public Uri ResponseUri;

        public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.79 Safari/537.36";

        public WebHeaderCollection WebHeaderCollection;

        public static HttpWebRequest BuildRequest(string method, string uri)
        {
            // Set up request
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.UserAgent = UserAgent;
            request.Method = method;
            request.ContentType = "application/json";

            // Important, otherwise the WebRequest will try to auto-follow
            // 302 redirects without applying the authorization header to the
            // subsequent requests.
            request.AllowAutoRedirect = false;// AllowAutoRedirect;

            if (!string.IsNullOrEmpty(ServerInfo.Credential.JwtBearer))
            {
                // Construct HTTP Basic authorization header
                //var authPayload = string.Format("{0}:{1}", API_KEY_ID, API_KEY_SECRET);
                //var authPayloadEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(authPayload));
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + ServerInfo.Credential.JwtBearer);
            }

            request.Accept =
                "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

            return request;
        }

        private object DoPost(string url, string content = null, string referer = null, int responseStream = 0)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.KeepAlive = true;
                httpWebRequest.UserAgent = UserAgent;
                httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;
                httpWebRequest.Accept =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                httpWebRequest.Proxy = null;

                if (referer != null) httpWebRequest.Referer = referer;

                httpWebRequest.Headers["Accept-Language"] = "en-US,en;q=0.8";
                httpWebRequest.Timeout = 60000;

                if (!string.IsNullOrEmpty(ServerInfo.Credential.JwtBearer))
                {
                    // Construct HTTP Basic authorization header
                    //var authPayload = string.Format("{0}:{1}", API_KEY_ID, API_KEY_SECRET);
                    //var authPayloadEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(authPayload));
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + ServerInfo.Credential.JwtBearer);
                }

                if (!string.IsNullOrWhiteSpace(content))
                {
                    httpWebRequest.Method = "POST";
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    var bytes = Encoding.UTF8.GetBytes(content);
                    httpWebRequest.ContentLength = bytes.Length;
                    using (var stream = httpWebRequest.GetRequestStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }

                if (CookieContainer != null) httpWebRequest.CookieContainer = CookieContainer;

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                HttpStatusCode = httpWebResponse.StatusCode;
                object result = null;

                switch (responseStream)
                {
                    case 1:
                        result = httpWebResponse.GetResponseStream();
                        break;
                    case 0:
                        var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                        result = streamReader.ReadToEnd();
                        streamReader.Close();
                        httpWebResponse.Close();
                        break;
                }

                ResponseUri = httpWebResponse.ResponseUri;
                WebHeaderCollection = httpWebResponse.Headers;
                httpWebResponse.Close();

                return result;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public string Post(string url, string content = null, string referer = null)
        {
            try
            {
                return DoPost(url, content, referer)?.ToString();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }
        //public Image GetStream(string url, string content = null, string referer = null)
        //{
        //    try
        //    {
        //        var obj = (Stream)DoPost(url, content, referer, 1);
        //        var result = Image.FromStream(obj);
        //        obj.Close();

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = ex.Message;
        //    }

        //    return null;
        //}
    }
}
