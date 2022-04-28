using Newtonsoft.Json;

namespace ApiToJKN.Models.Requests
{
    public class SisaAntrianRequest
    {
        [JsonProperty("kodebooking")]
        public string KodeBooking { get; set; }
        public string TanggalPeriksa { get; internal set; }
    }
}