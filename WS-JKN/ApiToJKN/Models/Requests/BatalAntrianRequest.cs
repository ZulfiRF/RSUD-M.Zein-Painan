using Newtonsoft.Json;

namespace ApiToJKN.Models.Requests
{
    public class BatalAntrianRequest : KodeBookingRequest
    {
        /// <summary>
        /// test 123
        /// </summary>
        [JsonProperty("keterangan")]
        public string Keterangan { get; set; }
    }
}