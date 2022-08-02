using Newtonsoft.Json;
using ProfileChecker.Authentication;

namespace StressTestZminingAPi
{
    public class ApiHanlder
    {
        private int Count = 0;
        public ApiHanlder()
        {
            ServerInfo.LoginByAdmin();
        }
        public async Task GetCampaignCache(string id)
        {
            var requestUri = Global.CAMPAIGN_API_URL + Global.API_v1 + "campaignRunner/campaignCache?campaignid=" + id;
            var getCurrentTenantRequest = HttpClientEx.BuildRequest("GET", requestUri);
            using (var response = getCurrentTenantRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var tenantData = await reader.ReadToEndAsync();
                Console.WriteLine("GetCampaignCache " + Count);
                Count++;
            }
        }

        public async Task GetCampagnRunningByClient(string id)
        {
            var requestUri = Global.CAMPAIGN_API_URL + Global.API_v1 + "campaignRunner/test?campaignid=" + id;
            var getCurrentTenantRequest = HttpClientEx.BuildRequest("GET", requestUri);
            using (var response = getCurrentTenantRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var tenantData = await reader.ReadToEndAsync();
                Console.WriteLine("GetCampagnRunningByClient" + Count);
                Count++;
            }
        }
    }
}