using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using CloudflareDDNS.Cloudflare;
using CloudflareDDNS.Logs;

namespace CloudflareDDNS
{
    public partial class CloudflareDDNSService : ServiceBase
    {
        private readonly CloudflareApi _cloudflareApi;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILog _log;
        private string _lastIpAddress;
        private Timer _timer;

        public CloudflareDDNSService(ILog log)
        {
            InitializeComponent();

            _log = log;
            _cloudflareApi = new CloudflareApi(ConfigurationManager.AppSettings["api_key"], ConfigurationManager.AppSettings["email"]);
        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer(60 * 1000D); // 60000 milliseconds = 60 seconds
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElasped;

            try
            {
                _timer.Start();
                _log.WriteLine("Timer started.");
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
        }

        private async void TimerElasped(object sender, ElapsedEventArgs e)
        {
            try
            {
                var httpResponseMessage = await _httpClient.GetAsync("http://checkip.amazonaws.com");
                var output = await httpResponseMessage.Content.ReadAsStringAsync();
                var currentIpAddress = Regex.Replace(output, @"\t|\n|\r", string.Empty);

                ValidateIpAddress(currentIpAddress);

                // unchanged -> return
                if (_lastIpAddress != null && _lastIpAddress == currentIpAddress) return;
                
                if (_lastIpAddress != null) 
                    _log.WriteLine($"New ip address, {currentIpAddress}");

                await SyncAllDnsRecords(currentIpAddress);
                _lastIpAddress = currentIpAddress;
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
        }

        private static void ValidateIpAddress(string ipAddress)
        {
            var regex = new Regex(
                "((?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))(?![\\d])", // IPv4 IP Address
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (!regex.IsMatch(ipAddress))
                throw new ApplicationException($"Not recognized output from http://checkip.amazonaws.com. Output, {ipAddress}");
        }

        private async Task SyncAllDnsRecords(string currentIpAddress)
        {
            var allZones = await _cloudflareApi.GetAllZonesAsync();
            var zoneIds = allZones.Select(zr => zr.id).ToList();

            // Get all A records in different zones
            var allDnsARecords = new List<DNSRecord>();
            foreach (var zoneId in zoneIds)
            {
                var dnsARecords = await _cloudflareApi.GetAllDnsARecordsAsync(zoneId);
                allDnsARecords.AddRange(dnsARecords);
            }

            // Only records with different content will be updated
            var updatingDnsRecords = allDnsARecords.Where(adarr => adarr.content != currentIpAddress).ToList();
            foreach (var updatingDnsRecord in updatingDnsRecords)
            {
                var updatedDnsRecord = await _cloudflareApi.UpdateDnsRecordAsync(
                    updatingDnsRecord.zone_id, updatingDnsRecord.id,
                    updatingDnsRecord.name, currentIpAddress, updatingDnsRecord.ttl, updatingDnsRecord.proxied);
            }

            if (updatingDnsRecords.Any())
            {
                var updatedDnsRecordNames = updatingDnsRecords.Select(udr => udr.name).ToList();
                _log.WriteLine($"All records have been synced. {string.Join(", ", updatedDnsRecordNames)}");
            }
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _log.WriteLine("Timer stopped.");
        }
    }
}