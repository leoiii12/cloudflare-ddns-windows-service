using System;
using System.Collections.Generic;

namespace CloudflareDDNS.Cloudflare
{
    public class Owner
    {
        public string id { get; set; }
        public string email { get; set; }
        public string owner_type { get; set; }
    }

    public class Plan
    {
        public string id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string currency { get; set; }
        public string frequency { get; set; }
        public string legacy_id { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
    }

    public class PlanPending
    {
        public string id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string currency { get; set; }
        public string frequency { get; set; }
        public string legacy_id { get; set; }
        public bool is_subscribed { get; set; }
        public bool can_subscribe { get; set; }
    }

    public class Zone
    {
        public string id { get; set; }
        public string name { get; set; }
        public int development_mode { get; set; }
        public IList<string> original_name_servers { get; set; }
        public string original_registrar { get; set; }
        public string original_dnshost { get; set; }
        public DateTime created_on { get; set; }
        public DateTime modified_on { get; set; }
        public IList<string> name_servers { get; set; }
        public Owner owner { get; set; }
        public IList<string> permissions { get; set; }
        public Plan plan { get; set; }
        public PlanPending plan_pending { get; set; }
        public string status { get; set; }
        public bool paused { get; set; }
        public string type { get; set; }
    }

    public class GetZonesOutput
    {
        public bool success { get; set; }
        public IList<Error> errors { get; set; }
        public IList<Message> messages { get; set; }
        public IList<Zone> result { get; set; }
        public ResultInfo result_info { get; set; }
    }
}