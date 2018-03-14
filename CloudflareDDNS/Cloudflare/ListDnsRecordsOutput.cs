using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareDDNS.Cloudflare
{
    public class Data
    {
    }

    public class DNSRecord
    {
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public bool proxiable { get; set; }
        public bool proxied { get; set; }
        public int ttl { get; set; }
        public bool locked { get; set; }
        public string zone_id { get; set; }
        public string zone_name { get; set; }
        public DateTime created_on { get; set; }
        public DateTime modified_on { get; set; }
        public Data data { get; set; }
    }

    public class ListDnsRecordsOutput
    {
        public bool success { get; set; }
        public IList<Error> errors { get; set; }
        public IList<Message> messages { get; set; }
        public IList<DNSRecord> result { get; set; }
        public ResultInfo result_info { get; set; }
    }

}
