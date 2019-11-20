using Newtonsoft.Json;

namespace DO_NOT_REMEMBER
{
    public class Resource
    {
        [JsonRequired]
        public string id { get; set; }

        [JsonRequired]
        public string pw { get; set; }

        [JsonRequired]
        public string ord { get; set; }

        [JsonRequired]
        public string pth { get; set; }
    }
}
