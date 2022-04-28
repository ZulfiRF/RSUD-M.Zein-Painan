using Newtonsoft.Json;

namespace ApiToJKN.Models.Requests
{
    public class StatusAntrianRequest
    {
        [JsonProperty("kodepoli")]
        public string KodePoli { get; set; }
        [JsonProperty("kodedokter")]
        public string KodeDokter { get; set; }
        [JsonProperty("tanggalperiksa")]
        public string TanggalPeriksa { get; set; }
        [JsonProperty("jampraktek")]
        public string JamPraktek { get; set; }
    }
}