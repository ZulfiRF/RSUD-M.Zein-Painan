using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class StatusAntrianRespone
    {
        [JsonProperty(PropertyName = "response")]
        public StatusAntrianDetail Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }
}