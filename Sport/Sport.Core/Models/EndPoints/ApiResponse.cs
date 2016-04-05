using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sport.Core.Models.EndPoints
{
    public class ApiResponse<T>
    {
        [JsonProperty("successful")]
        public bool Successful { get; set; }

        [JsonProperty("errorDesc")]
        public JToken ErrorDesc { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("auth")]
        public Session Auth { get; set; }

        [JsonProperty("log")]
        public string Log { get; set; }
    }
}