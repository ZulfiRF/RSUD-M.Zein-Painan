using Newtonsoft.Json;

namespace ApiToJKN.Models.Responses
{
    public class AmbilAntrianRespone
    {
        [JsonProperty(PropertyName = "response")]
        public AmbilAntrianDetail Response { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }
}