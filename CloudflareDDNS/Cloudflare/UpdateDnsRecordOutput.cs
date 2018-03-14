using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareDDNS.Cloudflare
{
    public class UpdateDnsRecordOutput
    {
        public bool success { get; set; }
        public IList<Error> errors { get; set; }
        public IList<Message> messages { get; set; }
        public DNSRecord result { get; set; }
    }
}
