using Newtonsoft.Json;

namespace ApiToJKN.Models.Requests
{
    public class KodeBookingRequest
    {
        [JsonProperty("kodebooking")]
        public string KodeBooking { get; set; }
    }
}