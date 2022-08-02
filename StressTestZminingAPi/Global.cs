using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressTestZminingAPi
{
    public static class Global
    {
        public static string AccessToken = "";
        public static string CUSTOMER_API = "http://203.205.40.149:6060/us-customer-api";
        public static string API_v1 = "/api/v1";

        public static string SERVER_ADDRESS = "203.205.40.149:6060/";
        public static string BASE_URL = "http://" + SERVER_ADDRESS;
        public static string CAMPAIGN_API_URL = BASE_URL + "us-running-campaign-api/api/v1/";
    }
}
