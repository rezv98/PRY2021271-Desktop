using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitor.ApplicationImp.HistoryModels
{
    public class UrlRequest
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("browser")]
        public string Browser { get; set; }

        [JsonProperty("uri")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        // Time & Date as string because thats how you get them
        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
    }
}
