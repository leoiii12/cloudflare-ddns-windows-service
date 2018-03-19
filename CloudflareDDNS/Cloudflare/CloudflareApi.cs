using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudflareDDNS.Cloudflare
{
    public class CloudflareApi
    {
        private readonly HttpClient _httpClient;

        public CloudflareApi(string apiKey, string email)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30.0);
            _httpClient.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Email", email);
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Key", apiKey);
        }

        public async Task<IReadOnlyCollection<Zone>> GetAllZonesAsync()
        {
            var formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("status", "active"),
                new KeyValuePair<string, string>("page", "1"),
                new KeyValuePair<string, string>("per_page", "50")
            });

            var query = await formUrlEncodedContent.ReadAsStringAsync();
            var httpResponseMessage = await _httpClient.GetAsync("zones?" + query);

            var getZonesOutput = JsonConvert.DeserializeObject<GetZonesOutput>(await httpResponseMessage.Content.ReadAsStringAsync());
            if (!getZonesOutput.success) throw new ApplicationException($"{nameof(GetAllZonesAsync)} failed.");

            return getZonesOutput.result.ToList();
        }

        public async Task<IReadOnlyCollection<DNSRecord>> GetAllDnsARecordsAsync(string zoneId)
        {
            var formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("type", "A"),
                new KeyValuePair<string, string>("page", "1"),
                new KeyValuePair<string, string>("per_page", "100")
            });

            var query = await formUrlEncodedContent.ReadAsStringAsync();
            var httpResponseMessage = await _httpClient.GetAsync($"zones/{zoneId}/dns_records?" + query);

            var listDnsRecordsOutput = JsonConvert.DeserializeObject<ListDnsRecordsOutput>(await httpResponseMessage.Content.ReadAsStringAsync());
            if (!listDnsRecordsOutput.success) throw new ApplicationException($"{nameof(GetAllDnsARecordsAsync)} failed.");

            return listDnsRecordsOutput.result.ToList();
        }

        public async Task<DNSRecord> UpdateDnsRecordAsync(string zoneId, string dnsRecordId, string newName, string newContent, int newTtl, bool newProxied)
        {
            var jsonContent = JsonConvert.SerializeObject(new
            {
                type = "A",
                name = newName,
                content = newContent,
                ttl = newTtl,
                proxied = newProxied
            });
            var httpResponseMessage = await _httpClient.PutAsync($"zones/{zoneId}/dns_records/{dnsRecordId}", new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            var updateDnsRecordOutput = JsonConvert.DeserializeObject<UpdateDnsRecordOutput>(await httpResponseMessage.Content.ReadAsStringAsync());
            if (!updateDnsRecordOutput.success)
                throw new ApplicationException($"{nameof(GetAllDnsARecordsAsync)} failed. {JsonConvert.SerializeObject(updateDnsRecordOutput)}. {JsonConvert.SerializeObject(httpResponseMessage.RequestMessage)}");

            return updateDnsRecordOutput.result;
        }
    }
}