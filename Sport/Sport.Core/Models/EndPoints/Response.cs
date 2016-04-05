using Newtonsoft.Json;

namespace Sport.Core.Models.EndPoints
{
    public class Response<T>
    {
        [JsonProperty("successful")]
        public bool Successful { get; set; }

        [JsonProperty("errorDesc")]
        public string ErrorDesc { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
        
        [JsonProperty("log")]
        public string Log { get; set; }
    }
}