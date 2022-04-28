using Newtonsoft.Json;

namespace ApiToJKN.Models.Requests
{
    public class CheckinRequest : KodeBookingRequest
    {
        [JsonProperty("waktu")]
        public long Waktu { get; set; }
    }
}